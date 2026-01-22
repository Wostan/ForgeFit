using ForgeFit.MAUI.Models.Enums.WorkoutEnums;

namespace ForgeFit.MAUI.Models.DTOs.Workout;

public record WorkoutExerciseDto(
    string ExternalId,
    string Name,
    Uri? GifUrl,
    List<Muscle> TargetMuscles,
    List<BodyPart> BodyParts,
    List<Equipment> Equipment,
    List<Muscle> SecondaryMuscles,
    List<string> Instructions);
