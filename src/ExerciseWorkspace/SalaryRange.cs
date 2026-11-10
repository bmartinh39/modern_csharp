namespace ExerciseWorkspace;

// Learner starter: implement the constructor and Update. See EXERCISE.md.
public class SalaryRange
{    

    public decimal Minimum { get; private set; }
    public decimal Maximum { get; private set; }

    
    
    public SalaryRange(decimal minimum, decimal maximum)
    {
        ValidateBounds(minimum, maximum);

        Minimum = minimum;
        Maximum = maximum;
    }
    

    public void Update(decimal minimum, decimal maximum)
    {
        ValidateBounds(minimum, maximum);
        
        Minimum = minimum;
        Maximum = maximum;
    }

    private static void ValidateBounds(decimal minimum, decimal maximum)
    {
       
        if (minimum < 0)
            throw new ArgumentOutOfRangeException(nameof(minimum), minimum, "Minimum cannot be negative");
        if (maximum < 0)
            throw new ArgumentOutOfRangeException(nameof(maximum), maximum, "Maximum cannot be negative");
        if (minimum > maximum)
            throw new ArgumentException("Minimum must be less or equal than maximum");
    }
}
