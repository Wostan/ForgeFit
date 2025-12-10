using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutExerciseDto(
    string ExternalId,
    string Name,
    string? GifUrl,
    List<Muscle> TargetMuscles,
    List<BodyPart> BodyParts,
    List<Equipment> Equipment,
    List<Muscle> SecondaryMuscles,
    List<string> Instructions);
