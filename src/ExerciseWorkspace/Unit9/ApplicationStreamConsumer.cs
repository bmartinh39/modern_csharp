namespace ExerciseWorkspace.Unit9;

public static class ApplicationStreamConsumer
{
    // Unit 9: Consume a borrowed asynchronous sequence without buffering it first.
    // - Process each entry once, in order, awaiting its callback before requesting
    //   the next entry. Return the count of successfully processed entries.
    // - Forward the exact caller token to enumeration and each callback.
    // - Empty input returns 0. Producer/callback faults and cancellation propagate;
    //   do not continue or request more items after a failure.
    // - Release the enumerator on all exits; await foreach handles this lifetime.
    // Inputs/items are non-null. No concurrency, Task.Run, blocking or detached work.
    // Reminder (teach first): await foreach reads items as they arrive;
    // source.WithCancellation(token) supplies the token to GetAsyncEnumerator.
    // This is a consumer task: writing an async iterator is not required.
    // Test: dotnet test --filter FullyQualifiedName~ApplicationStreamConsumerTests
    public static Task<int> ProcessAsync(
        IAsyncEnumerable<ApplicationEntry> source,
        Func<ApplicationEntry, CancellationToken, Task> processAsync,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
