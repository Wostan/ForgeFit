namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutExerciseSearchDto(
    string ExternalId,
    string Name,
    Uri? GifUrl);