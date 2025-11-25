using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutExerciseSearchDto(
    string ExternalId,
    string Name,
    List<Muscle> TargetMuscles,
    Uri? GifUrl);