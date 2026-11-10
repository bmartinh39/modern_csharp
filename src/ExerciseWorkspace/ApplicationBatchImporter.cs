namespace ExerciseWorkspace;

// Unit 7 integrated task: Import a batch of text files with clear failure boundaries.
// Implement Import; reuse ApplicationTextFile.LoadText for reading/reader ownership.
// - Reject null arguments with ArgumentNullException naming the argument.
// - Before reading or calling callbacks, validate every path: null/empty/whitespace
//   entries throw ArgumentException naming paths. string.IsNullOrWhiteSpace checks these.
// - Process paths in order (including duplicates). Pass each file's text to importText.
// - Return the number of importText calls that completed successfully; empty batch -> 0.
// - If reading a source throws FileNotFoundException, call reportMissing with that path
//   and continue. Other read failures propagate; do not turn them into missing files.
// - ALL callback failures propagate unchanged and stop the batch, even if a callback
//   throws FileNotFoundException. Earlier completed imports are not rolled back.
// - Do not change existing learner methods or tests. No async or parsing is required.
// Reminders: Action<string> accepts one string; FileNotFoundException and
// DirectoryNotFoundException both derive from IOException (neither derives from the other).
// Test: dotnet test --filter FullyQualifiedName~ApplicationBatchImporterTests
public static class ApplicationBatchImporter
{

    private static void ValidatePaths(IReadOnlyList<string> paths)
    {
        foreach(var path in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path was empty or null", nameof(paths));
        }
    }

    public static int Import(
        IReadOnlyList<string> paths,
        Action<string> importText,
        Action<string> reportMissing)
    {
        ArgumentNullException.ThrowIfNull(paths);
        ArgumentNullException.ThrowIfNull(importText);
        ArgumentNullException.ThrowIfNull(reportMissing);
        ValidatePaths(paths);

        var succesful = 0;

        foreach(var path in paths)
        {
            var content = "";
            try
            {               
                content = ApplicationTextFile.LoadText(path);                                
            }
            catch(FileNotFoundException)
            {
                reportMissing(path);
                continue;
            }   

            importText(content); 
            succesful++;                  
        }    

        return succesful;        
    }
}
