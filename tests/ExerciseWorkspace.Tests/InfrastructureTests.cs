using System.Reflection;

namespace ExerciseWorkspace.Tests;

// Infrastructure verification only; this does not assess learner knowledge.
public class InfrastructureTests
{
    [Fact]
    [Trait("Category", "Infrastructure")]
    public void Source_project_is_available_to_the_test_runner()
    {
        var assembly = Assembly.Load("ExerciseWorkspace");

        Assert.Equal("ExerciseWorkspace", assembly.GetName().Name);
        Assert.NotNull(assembly.EntryPoint);
    }
}
