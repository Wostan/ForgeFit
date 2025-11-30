using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutExerciseSearchResponse(
    string ExternalId,
    string Name,
    List<Muscle> TargetMuscles,
    Uri? GifUrl);