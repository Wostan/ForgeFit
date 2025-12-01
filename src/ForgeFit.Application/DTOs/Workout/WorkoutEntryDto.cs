namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutEntryDto(
    Guid Id,
    Guid WorkoutProgramId,
    TimeOnly Start,
    TimeOnly End,
    List<PerformedExerciseDto> PerformedExercises,
    double TotalVolume,
    double TotalReps
);
