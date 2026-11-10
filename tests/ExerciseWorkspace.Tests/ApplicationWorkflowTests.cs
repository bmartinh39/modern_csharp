using ExerciseWorkspace;

namespace ExerciseWorkspace.Tests;

public class ApplicationWorkflowTests
{
    [Fact]
    public void New_application_is_interested()
    {
        Assert.Equal(ApplicationStatus.Interested, new JobApplication().CurrentStatus);
    }

    [Fact]
    public void Interested_application_can_be_applied()
    {
        var application = new JobApplication();

        application.Apply();

        Assert.Equal(ApplicationStatus.Applied, application.CurrentStatus);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Interested_or_applied_application_can_be_rejected(bool applyFirst)
    {
        var application = new JobApplication();
        if (applyFirst)
            application.Apply();

        application.Reject();

        Assert.Equal(ApplicationStatus.Rejected, application.CurrentStatus);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Applying_processed_application_fails_without_changing_state(bool rejectFirst)
    {
        var application = new JobApplication();
        if (rejectFirst)
            application.Reject();
        else
            application.Apply();
        var previous = application.CurrentStatus;

        Assert.Throws<InvalidOperationException>(() => application.Apply());

        Assert.Equal(previous, application.CurrentStatus);
    }

    [Fact]
    public void Rejected_application_cannot_be_rejected_again()
    {
        var application = new JobApplication();
        application.Reject();

        Assert.Throws<InvalidOperationException>(() => application.Reject());

        Assert.Equal(ApplicationStatus.Rejected, application.CurrentStatus);
    }

    [Fact]
    public void Inbox_accepts_an_application()
    {
        var inbox = new ApplicationInbox();

        inbox.Add(new JobApplication());

        Assert.Equal(1, inbox.Count);
    }

    [Fact]
    public void Inbox_rejects_null_without_changing_existing_contents()
    {
        var inbox = new ApplicationInbox();
        inbox.Add(new JobApplication());

        var error = Assert.Throws<ArgumentNullException>(() => inbox.Add(null!));

        Assert.Equal("application", error.ParamName);
        Assert.Equal(1, inbox.Count);
    }
}
