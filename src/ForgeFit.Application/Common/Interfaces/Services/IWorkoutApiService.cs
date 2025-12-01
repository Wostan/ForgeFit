using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IWorkoutApiService
{
    Task<List<WorkoutExerciseSearchResponse>> SearchAsync(
        string query,
        List<Muscle>? muscles,
        List<BodyPart>? bodyParts,
        List<Equipment>? equipment,
        int pageNumber = 1,
        int pageSize = 20);

    Task<WorkoutExerciseDto> GetByIdAsync(string id);
}
