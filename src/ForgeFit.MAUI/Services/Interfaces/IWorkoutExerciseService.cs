using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.WorkoutEnums;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IWorkoutExerciseService
{
    Task<ServiceResponse<List<WorkoutExerciseSearchResponse>>> SearchExercisesAsync(
        string? query,
        List<Muscle>? muscles = null,
        List<BodyPart>? bodyParts = null,
        List<Equipment>? equipment = null,
        int pageNumber = 1,
        int pageSize = 20);

    Task<ServiceResponse<WorkoutExerciseDto>> GetExerciseByIdAsync(string id);
}
