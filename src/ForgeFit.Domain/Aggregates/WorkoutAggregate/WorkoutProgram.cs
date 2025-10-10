using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutProgram : EntityId, ITimeFields
{
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
    public ICollection<WorkoutExercisePlan> WorkoutExercisePlans { get; private set; }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("UserId cannot be empty.");

        UserId = userId;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Name cannot be null or whitespace.");
        if (name.Length > 50)
            throw new DomainValidationException("Name must be less than 50 characters long.");

        Name = name;
    }

    private void SetDescription(string? description)
    {
        if (description is not null && description.Length > 300)
            throw new DomainValidationException("Description must be less than 300 characters long.");

        Description = description;
    }

    private void SetWorkoutExercises(ICollection<WorkoutExercisePlan> workoutExercises)
    {
        WorkoutExercisePlans = workoutExercises ??
                               throw new DomainValidationException("Workout exercises cannot be null.");
    }

    public void AddWorkoutExercise(WorkoutExercisePlan workoutExercise)
    {
        if (workoutExercise is null)
            throw new DomainValidationException("Workout exercise cannot be null.");

        WorkoutExercisePlans.Add(workoutExercise);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveWorkoutExercise(WorkoutExercisePlan workoutExercise)
    {
        var item = WorkoutExercisePlans.FirstOrDefault(w => w.Id == workoutExercise.Id);
        if (item is null)
            throw new DomainValidationException("Workout exercise not found.");

        WorkoutExercisePlans.Remove(workoutExercise);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string name, string? description)
    {
        SetName(name);
        SetDescription(description);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsDeleted)
            throw new DomainValidationException("Workout program is already deleted.");

        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}