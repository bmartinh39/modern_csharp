using ExerciseWorkspace;

namespace ExerciseWorkspace.Tests;

public class SalaryRangeTests
{
    [Theory]
    [InlineData(50000, 70000)]
    [InlineData(0, 0)]
    [InlineData(60000, 60000)]
    public void Constructor_accepts_valid_bounds(int minimum, int maximum)
    {
        var range = new SalaryRange(minimum, maximum);

        Assert.Equal((decimal)minimum, range.Minimum);
        Assert.Equal((decimal)maximum, range.Maximum);
    }

    [Theory]
    [InlineData(-1, 70000, "minimum")]
    [InlineData(0, -1, "maximum")]
    public void Constructor_rejects_negative_bounds(int minimum, int maximum, string parameter)
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => new SalaryRange(minimum, maximum));

        Assert.Equal(parameter, error.ParamName);
    }

    [Fact]
    public void Constructor_rejects_reversed_nonnegative_bounds()
    {
        Assert.Throws<ArgumentException>(() => new SalaryRange(90000, 20000));
    }

    [Theory]
    [InlineData(80000, 90000)]
    [InlineData(10000, 20000)]
    [InlineData(0, 0)]
    public void Update_accepts_valid_replacement_bounds(int minimum, int maximum)
    {
        var range = new SalaryRange(50000, 70000);

        range.Update(minimum, maximum);

        Assert.Equal((decimal)minimum, range.Minimum);
        Assert.Equal((decimal)maximum, range.Maximum);
    }

    [Theory]
    [InlineData(-1, 70000, typeof(ArgumentOutOfRangeException))]
    [InlineData(0, -1, typeof(ArgumentOutOfRangeException))]
    [InlineData(90000, 20000, typeof(ArgumentException))]
    public void Failed_update_preserves_both_original_bounds(
        int minimum, int maximum, Type exceptionType)
    {
        var range = new SalaryRange(50000, 70000);

        var error = Assert.Throws(exceptionType, () => range.Update(minimum, maximum));

        if (error is ArgumentOutOfRangeException argumentError)
        {
            Assert.Equal(minimum < 0 ? "minimum" : "maximum", argumentError.ParamName);
        }

        Assert.Equal(50000m, range.Minimum);
        Assert.Equal(70000m, range.Maximum);
    }
}
