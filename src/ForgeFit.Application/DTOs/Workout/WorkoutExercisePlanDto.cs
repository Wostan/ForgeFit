namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutExercisePlanDto(
    Guid Id,
    Guid WorkoutProgramId,
    WorkoutExerciseDto WorkoutExercise, 
    List<WorkoutSetDto> WorkoutSets);