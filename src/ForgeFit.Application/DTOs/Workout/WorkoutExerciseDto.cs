using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutExerciseDto(
    string ExternalId,
    string Name,
    Uri? GifUrl,
    IReadOnlyCollection<Muscle> TargetMuscles,
    IReadOnlyCollection<BodyPart> BodyParts,
    IReadOnlyCollection<Equipment> Equipment,
    string? Instructions);