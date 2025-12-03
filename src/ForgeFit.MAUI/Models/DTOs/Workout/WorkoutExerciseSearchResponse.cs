using ForgeFit.MAUI.Models.Enums.WorkoutEnums;

namespace ForgeFit.MAUI.Models.DTOs.Workout;

public record WorkoutExerciseSearchResponse(
    string ExternalId,
    string Name,
    List<Muscle> TargetMuscles,
    Uri? GifUrl);
