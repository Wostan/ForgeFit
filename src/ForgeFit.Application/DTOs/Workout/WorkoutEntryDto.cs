namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutEntryDto( 
    Guid Id,
    Guid WorkoutProgramId,
    DateTime Start,
    DateTime End,
    double TotalVolume,
    double TotalReps,
    double TotalWeightMoved
    );