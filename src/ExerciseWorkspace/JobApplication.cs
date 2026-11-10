namespace ExerciseWorkspace;

// Unit 7, activity U7-F03: Protect application state (sections 4–10, 21–22, 30).
// - A new application starts Interested. Keep CurrentStatus externally read-only.
// - Apply allows only Interested -> Applied.
// - Reject allows Interested -> Rejected or Applied -> Rejected.
// - Invalid transitions throw InvalidOperationException and leave state unchanged.
// - No custom exception is needed: callers do not distinguish transition failures here.
// Implement Apply and Reject; keep the signatures. No catch blocks are needed.
// Test: dotnet test --filter FullyQualifiedName~ApplicationWorkflowTests
public enum ApplicationStatus { Interested, Applied, Rejected }

public class JobApplication
{
    public ApplicationStatus CurrentStatus { get; private set; } = ApplicationStatus.Interested;

    public void Apply()
    {
        if (CurrentStatus != ApplicationStatus.Interested)
            throw new InvalidOperationException("Application must be in Interested state to be applied");
        
        CurrentStatus = ApplicationStatus.Applied;
    }

    public void Reject()
    {
        if (CurrentStatus == ApplicationStatus.Rejected)
            throw new InvalidOperationException("Application cannot be in rejected state to be rejected");

        CurrentStatus = ApplicationStatus.Rejected;
    }
}

// Same activity: Complete Add to store the supplied application.
// - Reject null with ArgumentNullException naming application, before changing the list.
// - Use ArgumentNullException.ThrowIfNull(application), covered in section 21.
// - Valid input increases Count by one. Duplicate handling is outside this task.
public class ApplicationInbox
{
    private readonly List<JobApplication> applications = [];

    public int Count => applications.Count;

    public void Add(JobApplication application)    
    {
        ArgumentNullException.ThrowIfNull(application);
        applications.Add(application);
    }
}
