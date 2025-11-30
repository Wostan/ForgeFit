using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutProgram : Entity, ITimeFields
{
    private readonly List<WorkoutExercisePlan> _workoutExercisePlans = [];
    
    internal WorkoutProgram(
        Guid userId,
        string name,
        string? description,
        ICollection<WorkoutExercisePlan> workoutExercises)
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

    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; private set; }
    public IReadOnlyCollection<WorkoutExercisePlan> WorkoutExercisePlans => _workoutExercisePlans.AsReadOnly();
    
    public static WorkoutProgram Create(
        Guid userId,
        string name,
        string? description,
        ICollection<WorkoutExercisePlan> workoutExercises)
    {
        return new WorkoutProgram(userId, name, description, workoutExercises);
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("UserId cannot be empty.");
        }

        UserId = userId;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Name cannot be null or whitespace.");
        }
        if (name.Length > 50)
        {
            throw new DomainValidationException("Name must be less than 50 characters long.");
        }

        Name = name;
    }

    private void SetDescription(string? description)
    {
        if (description is not null && description.Length > 300)
        {
            throw new DomainValidationException("Description must be less than 300 characters long.");
        }

        Description = description;
    }

    private void SetWorkoutExercises(ICollection<WorkoutExercisePlan> workoutExercises)
    {
        if (workoutExercises is null)
        {
            throw new DomainValidationException("Workout exercises cannot be null.");
        }

        _workoutExercisePlans.Clear();
        
        if (workoutExercises.Count != 0)
        {
            _workoutExercisePlans.AddRange(workoutExercises);
        }
    }

    public void AddWorkoutExercises(ICollection<WorkoutExercisePlan> workoutExercises)
    {
        if (workoutExercises is null)
        {
            throw new DomainValidationException("Workout exercises cannot be null.");
        }
        
        _workoutExercisePlans.AddRange(workoutExercises);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveWorkoutExercise(WorkoutExercisePlan workoutExercise)
    {
        var item = WorkoutExercisePlans.FirstOrDefault(w => w.Id == workoutExercise.Id);
        if (item == null)
        {
            throw new DomainValidationException("Workout exercise not found.");
        }

        _workoutExercisePlans.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Update(
        string name, 
        string? description,
        ICollection<WorkoutExercisePlan> workoutExercises)
    {
        SetName(name);
        SetDescription(description);
        SetWorkoutExercises(workoutExercises);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsDeleted)
        {
            throw new DomainValidationException("Workout program is already deleted.");
        }

        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}