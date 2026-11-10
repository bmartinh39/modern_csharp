namespace ExerciseWorkspace;

// Unit 7: Run one import attempt using catch filters, propagation and finally.
// Implement Run:
// - Call import() exactly once. Return true on success; do not retry.
// - Catch IOException only when handleIoFailure is true; return false. Use when.
// - Let all other failures propagate, including IOException when the flag is false.
// - Use finally to call onFinished() once after every attempt, on success or failure.
// Assume both actions are non-null and onFinished never throws.
// Action is a no-argument, void-returning delegate; invoke it like a method.
// Test: dotnet test --filter FullyQualifiedName~ImportAttemptTests
public static class ImportAttempt
{
    public static bool Run(Action import, Action onFinished, bool handleIoFailure)
    {
        try
        {
            import();
            return true;    
        }
        catch (IOException) when (handleIoFailure)
        {            
            return false;
        }
        finally
        {
            onFinished();
        }            
    }
}
