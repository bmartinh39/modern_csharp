using ExerciseWorkspace;

namespace ExerciseWorkspace.Tests;

// Each test owns a unique temporary file and deletes only that file afterward.
public class ApplicationTextFileTests : IDisposable
{
    private readonly string path = Path.Combine(
        Path.GetTempPath(), $"modern-csharp-unit7-{Guid.NewGuid():N}.txt");

    [Theory]
    [InlineData("Company: Acme\nRole: Backend developer")]
    [InlineData("")]
    [InlineData("C# — España\r\nSecond line\r\n")]
    public void Reads_exact_text_and_releases_file(string content)
    {
        File.WriteAllText(path, content);

        Assert.Equal(content, ApplicationTextFile.LoadText(path));

        // On Windows an undisposed StreamReader prevents this exclusive open.
        using var exclusive = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.True(exclusive.CanRead);
    }

    [Fact]
    public void Missing_file_failure_propagates_instead_of_returning_empty_text()
    {
        Assert.Throws<FileNotFoundException>(() => ApplicationTextFile.LoadText(path));
    }

    public void Dispose()
    {
        File.Delete(path);
    }
}
