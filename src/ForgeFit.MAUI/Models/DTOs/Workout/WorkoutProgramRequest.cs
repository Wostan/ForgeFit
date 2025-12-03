namespace ForgeFit.MAUI.Models.DTOs.Workout;

public record WorkoutProgramRequest(
    string Name,
    string? Description,
    List<WorkoutExercisePlanDto> WorkoutExercisePlans);
