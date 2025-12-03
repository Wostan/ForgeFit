namespace ForgeFit.MAUI.Models.DTOs.Workout;

public record WorkoutProgramResponse(
    Guid Id,
    string Name,
    string? Description,
    List<WorkoutExercisePlanDto> WorkoutExercisePlans);
