namespace ExerciseWorkspace.Unit9;

public sealed record ApplicationEntry(Guid Id, Guid CompanyId, string Position);
public sealed record CompanyInfo(Guid Id, string Name);
public sealed record RecruiterInfo(string Name);
public sealed record ApplicationDetails(
    ApplicationEntry Application, CompanyInfo? Company, RecruiterInfo? Recruiter);

// Supplied infrastructure: calls return tasks immediately, may still be pending,
// and may complete successfully, fault, or observe the supplied cancellation token.
// Implementations in this exercise return tasks rather than throwing synchronously.
public interface IApplicationDetailsSource
{
    Task<ApplicationEntry?> FindApplicationAsync(Guid id, CancellationToken token);
    Task<CompanyInfo?> LoadCompanyAsync(Guid companyId, CancellationToken token);
    Task<RecruiterInfo?> LoadRecruiterAsync(Guid applicationId, CancellationToken token);
}

// Unit 9: Compose an asynchronous application-details workflow.
// - Reject a null source in the constructor (ArgumentNullException, source).
// - A pre-cancelled token stops the workflow before any source call.
// - Find the application first. Return null if absent; do not call child lookups.
// - Once found, start company and recruiter lookups before awaiting either result.
// - Return the original application and lookup results, including null child results.
// - Complete only when both started lookups finish, even when one fails. Unexpected
//   failures propagate; do not convert faults/cancellation into a missing result.
// - Pass the caller's exact token to every source call. Cancellation is cooperative;
//   this service does not own a CancellationTokenSource or cancel sibling operations.
// - No .Result, .Wait(), Task.Run, async void or detached work. Use Task.WhenAll.
// All Guid values are valid; source data is assumed internally consistent.
// Test: dotnet test --filter FullyQualifiedName~ApplicationDetailsServiceTests
public sealed class ApplicationDetailsService
{
    private readonly IApplicationDetailsSource _source;

    public ApplicationDetailsService(IApplicationDetailsSource source)
    {
        _source = source;
    }

    public Task<ApplicationDetails?> GetApplicationDetailsAsync(
        Guid applicationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
