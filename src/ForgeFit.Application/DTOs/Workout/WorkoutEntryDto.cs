namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutEntryDto( 
    Guid Id,
    Guid WorkoutProgramId,
    DateTime Start,
    DateTime End,
    List<PerformedExerciseDto> Exercises,
    double TotalVolume,
    double TotalReps
    );