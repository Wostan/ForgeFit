using System.Text.Json.Serialization;

namespace ForgeFit.Infrastructure.Services.ExerciseDBApi.Models;

internal record ExerciseDbResponse(
    [property: JsonPropertyName("success")]
    bool Success,
    [property: JsonPropertyName("metadata")]
    object? Metadata,
    [property: JsonPropertyName("data")] List<ExerciseDbItem> Data
);

internal record ExerciseDbSingleResponse(
    [property: JsonPropertyName("success")]
    bool Success,
    [property: JsonPropertyName("data")] ExerciseDbItem Data
);

internal record ExerciseDbItem(
    [property: JsonPropertyName("exerciseId")]
    string ExerciseId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("gifUrl")] string GifUrl,
    [property: JsonPropertyName("targetMuscles")]
    List<string> TargetMuscles,
    [property: JsonPropertyName("bodyParts")]
    List<string> BodyParts,
    [property: JsonPropertyName("equipments")]
    List<string> Equipments,
    [property: JsonPropertyName("secondaryMuscles")]
    List<string> SecondaryMuscles,
    [property: JsonPropertyName("instructions")]
    List<string> Instructions
);
