namespace ForgeFit.Application.DTOs.Workout;

public record PerformedExerciseDto(
    WorkoutExerciseDto ExerciseSnapshot, 
    List<PerformedSetDto> Sets
);