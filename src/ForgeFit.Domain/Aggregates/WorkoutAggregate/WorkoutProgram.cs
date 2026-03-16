using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutProgram : Entity, ITimeFields
{
    #region Private Fields
    private readonly List<WorkoutExercisePlan> _workoutExercisePlans = [];
    private readonly List<WorkoutEntry> _workoutEntries = [];
    #endregion

    #region Constructors
    internal WorkoutProgram(
        Guid userId,
        string name,
        string? description,
        ICollection<WorkoutExercisePlan> workoutExercises
    )
    {
        SetUserId(userId);
        SetName(name);
        SetDescription(description);
        SetWorkoutExercises(workoutExercises);
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutProgram()
    {
    }
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Navigation Properties
    public User User { get; private set; }
    public IReadOnlyCollection<WorkoutExercisePlan> WorkoutExercisePlans => _workoutExercisePlans.AsReadOnly();
    public IReadOnlyCollection<WorkoutEntry> WorkoutEntries => _workoutEntries.AsReadOnly();
    #endregion

    #region Factory Methods
    public static WorkoutProgram Create(
        Guid userId,
        string name,
        string? description,
        ICollection<WorkoutExercisePlan> workoutExercises
    )
    {
        return new WorkoutProgram(userId, name, description, workoutExercises);
    }
    #endregion

    #region Domain Methods
    public void UpdateDetails(string name, string? description)
    {
        SetName(name);
        SetDescription(description);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddExercisePlan(WorkoutExercisePlan plan)
    {
        if (plan is null) throw new DomainValidationException("Plan cannot be null.");
        if (_workoutExercisePlans.Any(p => p.Id == plan.Id)) 
            throw new DomainValidationException("Exercise plan already exists.");
        
        _workoutExercisePlans.Add(plan);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveExercisePlan(WorkoutExercisePlan plan)
    {
        if (plan is null) throw new DomainValidationException("Plan cannot be null.");
        
        _workoutExercisePlans.Remove(plan);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsDeleted) throw new DomainValidationException("Workout program is already deleted.");
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Private Setters
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        UserId = userId;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainValidationException("Name cannot be null or whitespace.");
        if (name.Length > DomainConstants.ValidationLimits.MaxWorkoutProgramNameLength) throw new DomainValidationException($"Name must be less than {DomainConstants.ValidationLimits.MaxWorkoutProgramNameLength} characters long.");
        Name = name;
    }

    private void SetDescription(string? description)
    {
        if (description is not null && description.Length > DomainConstants.ValidationLimits.MaxWorkoutProgramDescriptionLength)
            throw new DomainValidationException($"Description must be less than {DomainConstants.ValidationLimits.MaxWorkoutProgramDescriptionLength} characters long.");
        Description = description;
    }

    private void SetWorkoutExercises(ICollection<WorkoutExercisePlan> workoutExercises)
    {
        if (workoutExercises is null) throw new DomainValidationException("Workout exercises cannot be null.");
        
        if (workoutExercises.Count > DomainConstants.ValidationLimits.MaxExercisesPerProgram)
            throw new DomainValidationException($"Cannot exceed {DomainConstants.ValidationLimits.MaxExercisesPerProgram} exercises per program.");

        _workoutExercisePlans.Clear();
        _workoutExercisePlans.AddRange(workoutExercises);
    }
    #endregion
}
