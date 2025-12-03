namespace ForgeFit.MAUI.Models.DTOs.Workout;

public record WorkoutExercisePlanDto(
    Guid Id,
    Guid WorkoutProgramId,
    WorkoutExerciseDto WorkoutExercise,
    List<WorkoutSetDto> WorkoutSets);
