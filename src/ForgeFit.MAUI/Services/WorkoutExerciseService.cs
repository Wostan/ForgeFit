using System.Text;
using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.WorkoutEnums;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class WorkoutExerciseService(IApiService apiService) : IWorkoutExerciseService
{
    private const string BaseUrl = "/api/workout-api";

    public async Task<ServiceResponse<List<WorkoutExerciseSearchResponse>>> SearchExercisesAsync(
        string query,
        List<Muscle>? muscles = null,
        List<BodyPart>? bodyParts = null,
        List<Equipment>? equipment = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var sb = new StringBuilder($"{BaseUrl}/search?");
        
        sb.Append($"query={Uri.EscapeDataString(query)}");
        sb.Append($"&pageNumber={pageNumber}");
        sb.Append($"&pageSize={pageSize}");

        if (muscles != null && muscles.Count != 0)
        {
            foreach (var muscle in muscles) sb.Append($"&muscles={(int)muscle}");
        }

        if (bodyParts != null && bodyParts.Count != 0)
        {
            foreach (var part in bodyParts) sb.Append($"&bodyParts={(int)part}");
        }

        if (equipment != null && equipment.Count != 0)
        {
            foreach (var eq in equipment) sb.Append($"&equipment={(int)eq}");
        }

        return await apiService.GetAsync<List<WorkoutExerciseSearchResponse>>(sb.ToString());
    }

    public async Task<ServiceResponse<WorkoutExerciseDto>> GetExerciseByIdAsync(string id)
    {
        return await apiService.GetAsync<WorkoutExerciseDto>($"{BaseUrl}/{id}");
    }
}
