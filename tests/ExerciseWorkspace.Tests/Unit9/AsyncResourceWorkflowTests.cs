using ExerciseWorkspace.Unit9;

namespace ExerciseWorkspace.Tests.Unit9;

public class AsyncResourceWorkflowTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Stream_stays_alive_until_consumption_succeeds_or_faults(bool fail)
    {
        using var cancellation = new CancellationTokenSource();
        var stream = new TrackingStream();
        var gate = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var failure = new IOException("read failed");
        var opens = 0;
        var calls = 0;
        var operation = AsyncResourceWorkflow.ReadOwnedAsync(
            () => { opens++; return stream; },
            (received, token) =>
            {
                calls++;
                Assert.Same(stream, received);
                Assert.Equal(cancellation.Token, token);
                return gate.Task;
            }, cancellation.Token);
        try
        {
            Assert.Equal(1, opens);
            Assert.Equal(1, calls);
            Assert.Equal(0, stream.Disposals);
            Assert.False(operation.IsCompleted);
        }
        finally
        {
            if (fail) gate.TrySetException(failure);
            else gate.TrySetResult(17);
            try { await operation.WaitAsync(TimeSpan.FromSeconds(3)); }
            catch (IOException) when (fail) { }
        }
        if (fail)
            Assert.Same(failure, await Assert.ThrowsAsync<IOException>(() => operation));
        else
            Assert.Equal(17, await operation);
        Assert.Equal(1, stream.Disposals);
    }

    [Fact]
    public async Task Stream_is_disposed_after_cancellation()
    {
        using var cancellation = new CancellationTokenSource();
        var stream = new TrackingStream();
        var gate = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var operation = AsyncResourceWorkflow.ReadOwnedAsync(
            () => stream, (_, token) => gate.Task.WaitAsync(token), cancellation.Token);
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation.WaitAsync(TimeSpan.FromSeconds(3)));
        Assert.True(operation.IsCanceled);
        Assert.Equal(1, stream.Disposals);
    }

    [Fact]
    public async Task Synchronous_callback_failure_preserves_exception_and_disposes_stream()
    {
        var stream = new TrackingStream();
        var failure = new IOException("callback rejected stream");
        var error = await Assert.ThrowsAsync<IOException>(async () =>
            await AsyncResourceWorkflow.ReadOwnedAsync(() => stream, (_, _) => throw failure));
        Assert.Same(failure, error);
        Assert.Equal(1, stream.Disposals);
    }

    [Theory]
    [InlineData("success")]
    [InlineData("fault")]
    [InlineData("cancel")]
    public async Task Async_cleanup_finishes_before_the_workflow_completes(string outcome)
    {
        using var cancellation = new CancellationTokenSource();
        var lease = new AsyncLease();
        var processing = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var failure = new IOException("processing failed");
        var acquires = 0;
        var calls = 0;
        var operation = AsyncResourceWorkflow.ProcessOwnedAsync(
            () => { acquires++; return lease; },
            (received, token) =>
            {
                calls++;
                Assert.Same(lease, received);
                Assert.Equal(cancellation.Token, token);
                return processing.Task;
            }, cancellation.Token);
        try
        {
            Assert.Equal(1, acquires);
            Assert.Equal(1, calls);
            Assert.Equal(0, lease.Disposals);
            Assert.False(operation.IsCompleted);
            if (outcome == "fault") processing.SetException(failure);
            else if (outcome == "cancel")
            {
                cancellation.Cancel();
                processing.SetCanceled(cancellation.Token);
            }
            else processing.SetResult(23);
            await lease.CleanupStarted.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.False(operation.IsCompleted);
        }
        finally
        {
            processing.TrySetResult(23);
            lease.CleanupAllowed.TrySetResult(true);
        }
        if (outcome == "fault")
            Assert.Same(failure, await Assert.ThrowsAsync<IOException>(() => operation.WaitAsync(TimeSpan.FromSeconds(3))));
        else if (outcome == "cancel")
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation.WaitAsync(TimeSpan.FromSeconds(3)));
            Assert.True(operation.IsCanceled);
        }
        else Assert.Equal(23, await operation.WaitAsync(TimeSpan.FromSeconds(3)));
        Assert.Equal(1, lease.Disposals);
    }

    [Fact]
    public async Task Synchronous_processing_failure_still_awaits_async_cleanup()
    {
        var lease = new AsyncLease();
        var failure = new IOException("processing rejected resource");
        var operation = AsyncResourceWorkflow.ProcessOwnedAsync(() => lease, (_, _) => throw failure);
        try
        {
            await lease.CleanupStarted.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.False(operation.IsCompleted);
        }
        finally { lease.CleanupAllowed.TrySetResult(true); }
        Assert.Same(failure, await Assert.ThrowsAsync<IOException>(() => operation.WaitAsync(TimeSpan.FromSeconds(3))));
        Assert.Equal(1, lease.Disposals);
    }

    private sealed class TrackingStream : MemoryStream
    {
        public int Disposals { get; private set; }
        protected override void Dispose(bool disposing)
        {
            if (disposing) Disposals++;
            base.Dispose(disposing);
        }
    }

    private sealed class AsyncLease : IAsyncDisposable
    {
        public int Disposals { get; private set; }
        public TaskCompletionSource<bool> CleanupStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<bool> CleanupAllowed { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public async ValueTask DisposeAsync()
        {
            Disposals++;
            CleanupStarted.TrySetResult(true);
            await CleanupAllowed.Task;
        }
    }
}
