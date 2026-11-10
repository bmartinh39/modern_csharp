using ExerciseWorkspace;

namespace ExerciseWorkspace.Tests;

public class ApplicationBatchImporterTests : IDisposable
{
    private readonly string root = Path.Combine(
        Path.GetTempPath(), $"modern-csharp-batch-{Guid.NewGuid():N}");

    public ApplicationBatchImporterTests() => Directory.CreateDirectory(root);

    private string CreateFile(string name, string content)
    {
        var path = Path.Combine(root, name);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void Imports_exact_text_in_order_including_empty_files_and_duplicate_paths()
    {
        var first = CreateFile("first.txt", "First\nline");
        var empty = CreateFile("empty.txt", "");
        var imported = new List<string>();
        var missing = new List<string>();

        var count = ApplicationBatchImporter.Import(
            [first, empty, first], imported.Add, missing.Add);

        Assert.Equal(3, count);
        Assert.Equal(new[] { "First\nline", "", "First\nline" }, imported);
        Assert.Empty(missing);
        using var exclusive = File.Open(first, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.True(exclusive.CanRead);
    }

    [Fact]
    public void Missing_source_is_reported_in_order_and_does_not_stop_later_imports()
    {
        var first = CreateFile("first.txt", "first");
        var missing = Path.Combine(root, "missing.txt");
        var last = CreateFile("last.txt", "last");
        var events = new List<string>();

        var count = ApplicationBatchImporter.Import(
            [first, missing, last],
            text => events.Add($"import:{text}"),
            path => events.Add($"missing:{path}"));

        Assert.Equal(2, count);
        Assert.Equal(new[] { "import:first", $"missing:{missing}", "import:last" }, events);
    }

    [Fact]
    public void Null_batch_is_rejected_with_parameter_name()
    {
        var error = Assert.Throws<ArgumentNullException>(() =>
            ApplicationBatchImporter.Import(null!, _ => { }, _ => { }));

        Assert.Equal("paths", error.ParamName);
    }

    [Theory]
    [InlineData("importText")]
    [InlineData("reportMissing")]
    public void Null_callback_is_rejected_even_for_empty_batch(string parameter)
    {
        Action<string> valid = _ => { };

        var error = Assert.Throws<ArgumentNullException>(() => ApplicationBatchImporter.Import(
            [], parameter == "importText" ? null! : valid,
            parameter == "reportMissing" ? null! : valid));

        Assert.Equal(parameter, error.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Invalid_later_path_is_rejected_before_any_file_read_or_callback(string? invalid)
    {
        var missing = Path.Combine(root, "missing.txt");
        var imported = new List<string>();
        var reported = new List<string>();

        var error = Assert.Throws<ArgumentException>(() =>
            ApplicationBatchImporter.Import([missing, invalid!], imported.Add, reported.Add));

        Assert.Equal("paths", error.ParamName);
        Assert.Empty(imported);
        Assert.Empty(reported);
    }

    [Fact]
    public void Empty_batch_returns_zero_without_callbacks()
    {
        var calls = 0;

        var count = ApplicationBatchImporter.Import([], _ => calls++, _ => calls++);

        Assert.Equal(0, count);
        Assert.Equal(0, calls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Import_callback_failure_propagates_unchanged_without_missing_report_or_next_import(
        bool fileNotFound)
    {
        var first = CreateFile("first.txt", "first");
        var last = CreateFile("last.txt", "last");
        IOException failure = fileNotFound
            ? new FileNotFoundException("Failure in consumer")
            : new IOException("Failure in consumer");
        var calls = 0;
        var reported = new List<string>();

        var actual = Assert.Throws(failure.GetType(), () => ApplicationBatchImporter.Import(
            [first, last], _ => { calls++; throw failure; }, reported.Add));

        Assert.Same(failure, actual);
        Assert.Equal(1, calls);
        Assert.Empty(reported);
        using var exclusive = File.Open(first, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.True(exclusive.CanRead);
    }

    [Fact]
    public void Reporting_failure_propagates_unchanged_and_stops_batch()
    {
        var missing = Path.Combine(root, "missing.txt");
        var last = CreateFile("last.txt", "last");
        var failure = new InvalidOperationException("Reporting unavailable");
        var imports = 0;
        var reports = 0;

        var actual = Assert.Throws<InvalidOperationException>(() => ApplicationBatchImporter.Import(
            [missing, last], _ => imports++, _ => { reports++; throw failure; }));

        Assert.Same(failure, actual);
        Assert.Equal(0, imports);
        Assert.Equal(1, reports);
    }

    [Fact]
    public void Missing_directory_propagates_without_reporting_a_missing_file_or_continuing()
    {
        var inaccessiblePath = Path.Combine(root, "absent-directory", "file.txt");
        var last = CreateFile("last.txt", "last");
        var imports = 0;
        var reports = 0;

        Assert.Throws<DirectoryNotFoundException>(() => ApplicationBatchImporter.Import(
            [inaccessiblePath, last], _ => imports++, _ => reports++));

        Assert.Equal(0, imports);
        Assert.Equal(0, reports);
    }

    public void Dispose()
    {
        // Only this fixture's unique directory and its directly created files are removed.
        foreach (var file in Directory.EnumerateFiles(root))
            File.Delete(file);
        Directory.Delete(root);
    }
}
