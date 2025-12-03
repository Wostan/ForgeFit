namespace ForgeFit.MAUI.Models.DTOs.Workout;

public record PerformedExerciseDto(
    WorkoutExerciseDto ExerciseSnapshot,
    List<PerformedSetDto> Sets
);
