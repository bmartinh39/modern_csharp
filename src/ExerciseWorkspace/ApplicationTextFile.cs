namespace ExerciseWorkspace;

// Unit 7, activity U7-F07: Load text while owning its reader (sections 23–28, 30).
// Implement LoadText:
// - Create a StreamReader for path and return its entire content, including newlines.
// - Dispose your reader on every exit path. Use a using declaration.
// - Let read/open failures propagate; do not convert failures into empty text.
// - Empty files return an empty string. Assume path is non-null.
// API reminders: new StreamReader(path) opens a reader; reader.ReadToEnd() returns text.
// You own this reader. No borrowed stream, leaveOpen option or file-writing code is needed.
// Test: dotnet test --filter FullyQualifiedName~ApplicationTextFileTests
public static class ApplicationTextFile
{
    public static string LoadText(string path)
    {
        using StreamReader streamReader = new StreamReader(path);
        return streamReader.ReadToEnd();        
    }
}
