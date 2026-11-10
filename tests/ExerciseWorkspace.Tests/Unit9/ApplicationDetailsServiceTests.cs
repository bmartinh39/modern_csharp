using ExerciseWorkspace.Unit9;

namespace ExerciseWorkspace.Tests.Unit9;

public class ApplicationDetailsServiceTests
{
    private static readonly ApplicationEntry Application = new(Guid.NewGuid(), Guid.NewGuid(), "Developer");
    private static readonly CompanyInfo Company = new(Application.CompanyId, "Acme");
    private static readonly RecruiterInfo Recruiter = new("Sam");

    [Fact]
    public void Constructor_rejects_null_source()
    {
        var error = Assert.Throws<ArgumentNullException>(() => new ApplicationDetailsService(null!));
        Assert.Equal("source", error.ParamName);
    }

    [Fact]
    public async Task Missing_application_returns_null_without_child_calls()
    {
        var source = new Source { Find = (_, _) => Task.FromResult<ApplicationEntry?>(null) };
        Assert.Null(await new ApplicationDetailsService(source).GetApplicationDetailsAsync(Application.Id));
        Assert.Equal(["application"], source.Calls);
    }

    [Fact]
    public async Task Results_preserve_identity_and_use_the_correct_ids()
    {
        var source = new Source();
        var result = await new ApplicationDetailsService(source).GetApplicationDetailsAsync(Application.Id);
        Assert.NotNull(result);
        Assert.Same(Application, result.Application);
        Assert.Same(Company, result.Company);
        Assert.Same(Recruiter, result.Recruiter);
        Assert.Equal(Application.Id, source.ApplicationId);
        Assert.Equal(Application.CompanyId, source.CompanyId);
        Assert.Equal(Application.Id, source.RecruiterId);
    }

    [Fact]
    public async Task Missing_children_remain_null_in_a_successful_result()
    {
        var source = new Source
        {
            CompanyLoad = (_, _) => Task.FromResult<CompanyInfo?>(null),
            RecruiterLoad = (_, _) => Task.FromResult<RecruiterInfo?>(null)
        };
        var result = await new ApplicationDetailsService(source).GetApplicationDetailsAsync(Application.Id);
        Assert.NotNull(result);
        Assert.Null(result.Company);
        Assert.Null(result.Recruiter);
    }

    [Fact]
    public async Task Application_must_finish_before_children_but_children_overlap()
    {
        var application = Gate<ApplicationEntry?>();
        var company = Gate<CompanyInfo?>();
        var recruiter = Gate<RecruiterInfo?>();
        var source = new Source
        {
            Find = (_, _) => application.Task,
            CompanyLoad = (_, _) => company.Task,
            RecruiterLoad = (_, _) => recruiter.Task
        };
        var operation = new ApplicationDetailsService(source).GetApplicationDetailsAsync(Application.Id);
        try
        {
            Assert.Equal(["application"], source.Calls);
            application.SetResult(Application);
            await source.BothStarted.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.False(operation.IsCompleted);
            recruiter.SetResult(Recruiter);
            Assert.False(operation.IsCompleted);
            company.SetResult(Company);
            Assert.NotNull(await operation.WaitAsync(TimeSpan.FromSeconds(3)));
        }
        finally
        {
            application.TrySetResult(Application);
            company.TrySetResult(Company);
            recruiter.TrySetResult(Recruiter);
        }
    }

    [Fact]
    public async Task Every_lookup_receives_the_callers_token()
    {
        using var cancellation = new CancellationTokenSource();
        var source = new Source();
        await new ApplicationDetailsService(source).GetApplicationDetailsAsync(Application.Id, cancellation.Token);
        Assert.Equal(3, source.Tokens.Count);
        Assert.All(source.Tokens, token => Assert.Equal(cancellation.Token, token));
    }

    [Fact]
    public async Task Pre_cancelled_request_does_not_call_the_source()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var source = new Source();
        var operation = new ApplicationDetailsService(source).GetApplicationDetailsAsync(Application.Id, cancellation.Token);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation);
        Assert.True(operation.IsCanceled);
        Assert.Empty(source.Calls);
    }

    [Fact]
    public async Task Lookup_failure_propagates_without_child_calls()
    {
        var failure = new IOException("lookup unavailable");
        var source = new Source { Find = (_, _) => Task.FromException<ApplicationEntry?>(failure) };
        var error = await Assert.ThrowsAsync<IOException>(() =>
            new ApplicationDetailsService(source).GetApplicationDetailsAsync(Application.Id));
        Assert.Same(failure, error);
        Assert.Equal(["application"], source.Calls);
    }

    [Fact]
    public async Task Child_fault_does_not_finish_the_workflow_before_its_sibling()
    {
        var failure = new IOException("company unavailable");
        // Inline continuations make the post-fault completion assertion deterministic.
        var company = new TaskCompletionSource<CompanyInfo?>();
        var recruiter = Gate<RecruiterInfo?>();
        var source = new Source
        {
            CompanyLoad = (_, _) => company.Task,
            RecruiterLoad = (_, _) => recruiter.Task
        };
        var operation = new ApplicationDetailsService(source).GetApplicationDetailsAsync(Application.Id);
        try
        {
            await source.BothStarted.Task.WaitAsync(TimeSpan.FromSeconds(3));
            company.SetException(failure);
            Assert.False(operation.IsCompleted);
            recruiter.SetResult(Recruiter);
            var error = await Assert.ThrowsAsync<IOException>(() => operation.WaitAsync(TimeSpan.FromSeconds(3)));
            Assert.Same(failure, error);
        }
        finally
        {
            company.TrySetResult(Company);
            recruiter.TrySetResult(Recruiter);
        }
    }

    [Fact]
    public async Task Cancellation_during_child_work_is_propagated_as_cancellation()
    {
        using var cancellation = new CancellationTokenSource();
        var company = Gate<CompanyInfo?>();
        var recruiter = Gate<RecruiterInfo?>();
        var source = new Source
        {
            CompanyLoad = (_, token) => company.Task.WaitAsync(token),
            RecruiterLoad = (_, token) => recruiter.Task.WaitAsync(token)
        };
        var operation = new ApplicationDetailsService(source).GetApplicationDetailsAsync(Application.Id, cancellation.Token);
        try
        {
            await source.BothStarted.Task.WaitAsync(TimeSpan.FromSeconds(3));
            cancellation.Cancel();
            var error = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation.WaitAsync(TimeSpan.FromSeconds(3)));
            Assert.Equal(cancellation.Token, error.CancellationToken);
            Assert.True(operation.IsCanceled);
        }
        finally
        {
            company.TrySetResult(Company);
            recruiter.TrySetResult(Recruiter);
        }
    }

    private static TaskCompletionSource<T> Gate<T>() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private sealed class Source : IApplicationDetailsSource
    {
        public Func<Guid, CancellationToken, Task<ApplicationEntry?>> Find = (_, _) => Task.FromResult<ApplicationEntry?>(Application);
        public Func<Guid, CancellationToken, Task<CompanyInfo?>> CompanyLoad = (_, _) => Task.FromResult<CompanyInfo?>(Company);
        public Func<Guid, CancellationToken, Task<RecruiterInfo?>> RecruiterLoad = (_, _) => Task.FromResult<RecruiterInfo?>(Recruiter);
        public readonly List<string> Calls = [];
        public readonly List<CancellationToken> Tokens = [];
        public readonly TaskCompletionSource<bool> BothStarted = Gate<bool>();
        public Guid ApplicationId, CompanyId, RecruiterId;

        public Task<ApplicationEntry?> FindApplicationAsync(Guid id, CancellationToken token)
        {
            ApplicationId = id;
            Calls.Add("application");
            Tokens.Add(token);
            return Find(id, token);
        }

        public Task<CompanyInfo?> LoadCompanyAsync(Guid id, CancellationToken token)
        {
            CompanyId = id;
            MarkChild("company", token);
            return CompanyLoad(id, token);
        }

        public Task<RecruiterInfo?> LoadRecruiterAsync(Guid id, CancellationToken token)
        {
            RecruiterId = id;
            MarkChild("recruiter", token);
            return RecruiterLoad(id, token);
        }

        private void MarkChild(string name, CancellationToken token)
        {
            Calls.Add(name);
            Tokens.Add(token);
            if (Calls.Contains("company") && Calls.Contains("recruiter")) BothStarted.TrySetResult(true);
        }
    }
}
