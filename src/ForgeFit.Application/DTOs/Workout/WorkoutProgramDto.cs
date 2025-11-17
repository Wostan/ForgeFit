namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutProgramDto(
    Guid Id,
    string Name,
    string? Description,
    List<WorkoutExercisePlanDto> WorkoutExercisePlans);