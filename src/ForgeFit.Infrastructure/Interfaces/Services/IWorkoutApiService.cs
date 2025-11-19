using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Infrastructure.Interfaces.Services;

public interface IWorkoutApiService
{
    Task<List<WorkoutExerciseSearchDto>> SearchAsync(
        string query,
        List<Muscle>? muscles,
        List<BodyPart>? bodyParts,
        List<Equipment>? equipment,
        int pageNumber = 1,
        int pageSize = 20);
    
    Task<WorkoutExerciseDto> GetByIdAsync(string id);
}