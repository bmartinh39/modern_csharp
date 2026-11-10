using ExerciseWorkspace.Unit9;

namespace ExerciseWorkspace.Tests.Unit9;

public class ApplicationStreamConsumerTests
{
    private static readonly ApplicationEntry First = new(Guid.NewGuid(), Guid.NewGuid(), "First");
    private static readonly ApplicationEntry Second = new(Guid.NewGuid(), Guid.NewGuid(), "Second");

    [Fact]
    public async Task Items_are_processed_in_order_with_the_exact_token_and_enumerator_disposed()
    {
        using var cancellation = new CancellationTokenSource();
        var source = new Sequence(First, Second);
        var seen = new List<ApplicationEntry>();
        var count = await ApplicationStreamConsumer.ProcessAsync(source, (item, token) =>
        {
            Assert.Equal(cancellation.Token, token);
            seen.Add(item);
            return Task.CompletedTask;
        }, cancellation.Token);
        Assert.Equal(2, count);
        Assert.Equal(new[] { First, Second }, seen);
        Assert.Same(First, seen[0]);
        Assert.Equal(cancellation.Token, source.Token);
        Assert.Equal(1, source.Disposals);
    }

    [Fact]
    public async Task Empty_sequence_returns_zero_without_callbacks()
    {
        var source = new Sequence();
        var calls = 0;
        Assert.Equal(0, await ApplicationStreamConsumer.ProcessAsync(source, (_, _) =>
        {
            calls++;
            return Task.CompletedTask;
        }));
        Assert.Equal(0, calls);
        Assert.Equal(1, source.Disposals);
    }

    [Fact]
    public async Task Does_not_request_the_next_item_until_processing_finishes()
    {
        var source = new Sequence(First, Second);
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var operation = ApplicationStreamConsumer.ProcessAsync(source, (item, _) =>
        {
            if (item == First) { started.TrySetResult(true); return gate.Task; }
            return Task.CompletedTask;
        });
        try
        {
            await started.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal(1, source.Moves);
            Assert.False(operation.IsCompleted);
        }
        finally { gate.TrySetResult(true); }
        Assert.Equal(2, await operation.WaitAsync(TimeSpan.FromSeconds(3)));
    }

    [Fact]
    public async Task Callback_failure_propagates_without_requesting_more_items()
    {
        var source = new Sequence(First, Second);
        var failure = new IOException("save failed");
        var error = await Assert.ThrowsAsync<IOException>(() =>
            ApplicationStreamConsumer.ProcessAsync(source, (_, _) => Task.FromException(failure)));
        Assert.Same(failure, error);
        Assert.Equal(1, source.Moves);
        Assert.Equal(1, source.Disposals);
    }

    [Fact]
    public async Task Producer_failure_propagates_and_disposes_enumerator()
    {
        var failure = new IOException("source disconnected");
        var source = new Sequence(First) { FailureAfterItems = failure };
        var calls = 0;
        var error = await Assert.ThrowsAsync<IOException>(() =>
            ApplicationStreamConsumer.ProcessAsync(source, (_, _) => { calls++; return Task.CompletedTask; }));
        Assert.Same(failure, error);
        Assert.Equal(1, calls);
        Assert.Equal(1, source.Disposals);
    }

    [Fact]
    public async Task Cancellation_between_items_stops_enumeration_and_disposes_it()
    {
        using var cancellation = new CancellationTokenSource();
        var source = new Sequence(First, Second);
        var calls = 0;
        var operation = ApplicationStreamConsumer.ProcessAsync(source, (_, token) =>
        {
            Assert.Equal(cancellation.Token, token);
            calls++;
            cancellation.Cancel();
            return Task.CompletedTask;
        }, cancellation.Token);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation);
        Assert.True(operation.IsCanceled);
        Assert.Equal(1, calls);
        Assert.Equal(1, source.Disposals);
    }

    // Supplied probe: the learner consumes this contract, not this implementation.
    private sealed class Sequence(params ApplicationEntry[] items) : IAsyncEnumerable<ApplicationEntry>, IAsyncEnumerator<ApplicationEntry>
    {
        private int _index = -1;
        public int Moves { get; private set; }
        public int Disposals { get; private set; }
        public CancellationToken Token { get; private set; }
        public Exception? FailureAfterItems { get; init; }
        public ApplicationEntry Current => items[_index];
        public IAsyncEnumerator<ApplicationEntry> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            Token = cancellationToken;
            return this;
        }
        public ValueTask<bool> MoveNextAsync()
        {
            Moves++;
            Token.ThrowIfCancellationRequested();
            _index++;
            if (_index >= items.Length && FailureAfterItems is not null)
                return ValueTask.FromException<bool>(FailureAfterItems);
            return ValueTask.FromResult(_index < items.Length);
        }
        public ValueTask DisposeAsync()
        {
            Disposals++;
            return ValueTask.CompletedTask;
        }
    }
}
