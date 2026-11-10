using ExerciseWorkspace;

namespace ExerciseWorkspace.Tests;

public class ImportAttemptTests
{
    [Fact]
    public void Successful_import_returns_true_and_finishes_once_after_import()
    {
        var events = new List<string>();

        var result = ImportAttempt.Run(
            () => events.Add("import"),
            () => events.Add("finished"),
            handleIoFailure: false);

        Assert.True(result);
        Assert.Equal(new[] { "import", "finished" }, events);
    }

    [Fact]
    public void Handled_io_failure_returns_false_without_retrying_and_finishes_once()
    {
        var events = new List<string>();

        var result = ImportAttempt.Run(
            () => { events.Add("import"); throw new IOException("Unavailable"); },
            () => events.Add("finished"),
            handleIoFailure: true);

        Assert.False(result);
        Assert.Equal(new[] { "import", "finished" }, events);
    }

    [Fact]
    public void Unhandled_io_failure_propagates_original_exception_and_finishes_once()
    {
        var events = new List<string>();
        var failure = new IOException("Unavailable");

        var actual = Assert.Throws<IOException>(() => ImportAttempt.Run(
            () => { events.Add("import"); throw failure; },
            () => events.Add("finished"),
            handleIoFailure: false));

        Assert.Same(failure, actual);
        Assert.Equal(new[] { "import", "finished" }, events);
    }

    [Fact]
    public void Unrelated_failure_propagates_even_when_io_handling_is_enabled()
    {
        var events = new List<string>();
        var failure = new InvalidOperationException("Invalid state");

        var actual = Assert.Throws<InvalidOperationException>(() => ImportAttempt.Run(
            () => { events.Add("import"); throw failure; },
            () => events.Add("finished"),
            handleIoFailure: true));

        Assert.Same(failure, actual);
        Assert.Equal(new[] { "import", "finished" }, events);
    }
}
