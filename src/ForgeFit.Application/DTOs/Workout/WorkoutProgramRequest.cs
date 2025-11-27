namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutProgramRequest(
    string Name,
    string? Description,
    List<WorkoutExercisePlanDto> WorkoutExercisePlans);