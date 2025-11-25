using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Aggregates.GoalAggregate;

public class BodyGoal : Entity, ITimeFields
{
    internal BodyGoal(
        Guid userId,
        string title,
        string? description,
        Weight weightGoal,
        DateTime? dueDate,
        GoalType goalType,
        GoalStatus goalStatus)
    {
        SetUserId(userId);
        SetTitle(title);
        SetDescription(description);
        SetWeightGoal(weightGoal);
        SetDueDate(dueDate);
        SetGoalType(goalType);
        SetGoalStatus(goalStatus);
        CreatedAt = DateTime.UtcNow;
    }

    private BodyGoal()
    {
    }

    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Weight WeightGoal { get; private set; }
    public DateTime? DueDate { get; private set; }
    public GoalType GoalType { get; private set; }
    public GoalStatus GoalStatus { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; private set; }
    
    public static BodyGoal Create(
        Guid userId,
        string title,
        string? description,
        Weight weightGoal,
        DateTime? dueDate,
        GoalType goalType,
        GoalStatus goalStatus)
    {
        return new BodyGoal(userId, title, description, weightGoal, dueDate, goalType, goalStatus);
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("WorkoutProgramId cannot be empty.");

        UserId = userId;
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainValidationException("Title cannot be null or whitespace.");

        if (title.Length > 20)
            throw new DomainValidationException("Title must be less than 20 characters long.");

        Title = title;
    }

    private void SetDescription(string? description)
    {
        if (description is not null && description.Length > 100)
            throw new DomainValidationException("Description must be less than 100 characters long.");

        Description = description;
    }

    private void SetWeightGoal(Weight weightGoal)
    {
        WeightGoal = weightGoal;
    }

    private void SetDueDate(DateTime? dueDate)
    {
        if (dueDate is not null && dueDate < DateTime.UtcNow)
            throw new DomainValidationException("DueDate cannot be in the past.");

        DueDate = dueDate;
    }

    private void SetGoalType(GoalType goalType)
    {
        if (!Enum.IsDefined(goalType))
            throw new DomainValidationException("GoalType is not defined.");

        GoalType = goalType;
    }

    private void SetGoalStatus(GoalStatus goalStatus)
    {
        if (!Enum.IsDefined(goalStatus))
            throw new DomainValidationException("GoalStatus is not defined.");

        GoalStatus = goalStatus;
    }

    public void Complete()
    {
        if (GoalStatus != GoalStatus.InProgress)
            throw new DomainValidationException("BodyGoal is not in progress.");

        GoalStatus = GoalStatus.Completed;
    }

    public void Cancel()
    {
        if (GoalStatus != GoalStatus.InProgress)
            throw new DomainValidationException("BodyGoal is not in progress.");

        GoalStatus = GoalStatus.Cancelled;
    }

    public void Reopen()
    {
        if (GoalStatus != GoalStatus.Cancelled)
            throw new DomainValidationException("BodyGoal is not cancelled.");

        GoalStatus = GoalStatus.InProgress;
    }

    public void UpdateInfo(string title, string? desc, DateTime? dueDate)
    {
        SetTitle(title);
        SetDescription(desc);
        SetDueDate(dueDate);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateWeightGoal(Weight weightGoal)
    {
        if (WeightGoal.ToKg() == weightGoal.ToKg())
            throw new DomainValidationException("Weight goal is the same.");

        SetWeightGoal(weightGoal);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateGoalType(GoalType goalType)
    {
        SetGoalType(goalType);
        UpdatedAt = DateTime.UtcNow;
    }
}