namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutExercisePlanDto(
    Guid Id,
    WorkoutExerciseDto WorkoutExercise, 
    List<WorkoutSetDto> Sets);