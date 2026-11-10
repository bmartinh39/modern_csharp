namespace ExerciseWorkspace.Unit9;

public static class AsyncResourceWorkflow
{
    // Unit 9 + Unit 7: Debug premature disposal of an owned stream.
    // - Open once, invoke consumeAsync once with that stream and the exact token.
    // - Keep the stream alive until consumption completes; return its result.
    // - Dispose on success, fault, cancellation, or a synchronous callback throw.
    // - Preserve the original exception; no blocking or detached work.
    // Inputs/factory results are non-null; the stream's Dispose does not throw.
    // This starter deliberately contains a lifetime bug; investigate before editing.
    // Test: dotnet test --filter FullyQualifiedName~AsyncResourceWorkflowTests
    public static Task<int> ReadOwnedAsync(
        Func<Stream> open,
        Func<Stream, CancellationToken, Task<int>> consumeAsync,
        CancellationToken cancellationToken = default)
    {
        using var stream = open();
        return consumeAsync(stream, cancellationToken);
    }

    // Unit 9 + Unit 7: Own a resource requiring asynchronous cleanup.
    // - Acquire once and process once, forwarding the exact token and resource.
    // - Keep it alive through processing; await DisposeAsync on every exit path.
    // - Do not complete the returned task before cleanup completes.
    // - Preserve the processing result/original exception. Cleanup does not throw.
    // Inputs/results are non-null. No pre-cancellation policy beyond token forwarding.
    // Reminder: await using invokes IAsyncDisposable.DisposeAsync and awaits cleanup;
    // ValueTask is its supplied return type here, not an API you need to design.
    // Same filtered test command as above; explanation precedes assignment in chat.
    public static Task<int> ProcessOwnedAsync(
        Func<IAsyncDisposable> acquire,
        Func<IAsyncDisposable, CancellationToken, Task<int>> processAsync,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
