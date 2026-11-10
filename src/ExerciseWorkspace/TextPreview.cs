namespace ExerciseWorkspace;

// Deliberately flawed debugging exercise. See RESOURCE-EXERCISE.md.
public static class TextPreview
{
    // Borrows source: callers retain ownership.
    public static string ReadFirstLine(Stream source)
    {
        var reader = new StreamReader(source);
        return reader.ReadLine()
            ?? throw new InvalidDataException("The input is empty.");
    }

    // Takes ownership of the stream returned by openStream.
    public static string LoadFirstLine(Func<Stream> openStream)
    {
        using var stream = openStream();
        var line = ReadFirstLine(stream);        
        return line;
    }
}
