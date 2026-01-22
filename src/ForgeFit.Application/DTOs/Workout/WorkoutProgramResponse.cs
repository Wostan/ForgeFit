namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutProgramResponse(
    Guid Id,
    string Name,
    string? Description,
    List<WorkoutExercisePlanDto> WorkoutExercisePlans);
