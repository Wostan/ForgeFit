using System.Net.Http.Json;
using System.Text.RegularExpressions;
using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Infrastructure.Services.ExerciseDBApi.Models;

namespace ForgeFit.Infrastructure.Services.ExerciseDBApi;

public partial class WorkoutApiService(HttpClient client) : IWorkoutApiService
{
    public async Task<List<WorkoutExerciseSearchDto>> SearchAsync(
        string query, 
        List<Muscle>? muscles, 
        List<BodyPart>? bodyParts, 
        List<Equipment>? equipment, 
        int pageNumber = 1, 
        int pageSize = 20)
    { 
        var offset = (pageNumber - 1) * pageSize;
        var queryParams = new List<string>
        {
            $"offset={offset}",
            $"limit={pageSize}"
        };

        if (!string.IsNullOrWhiteSpace(query))
        {
            queryParams.Add($"search={Uri.EscapeDataString(query)}");
        }

        if (muscles is { Count: > 0 })
            queryParams.Add($"muscles={EnumsToApiString(muscles)}");

        if (bodyParts is { Count: > 0 })
            queryParams.Add($"bodyParts={EnumsToApiString(bodyParts)}");
        
        if (equipment is { Count: > 0 })
            queryParams.Add($"equipment={EnumsToApiString(equipment)}");
        
        var requestUrl = $"exercises/filter?{string.Join("&", queryParams)}"; 

        var response = await client.GetAsync(requestUrl);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to retrieve exercises.");
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<ExerciseDbResponse>();

        if (apiResponse is null || !apiResponse.Success)
        {
            return [];
        }

        return apiResponse.Data.Select(item => new WorkoutExerciseSearchDto(
            item.ExerciseId,
            item.Name,
            ParseEnums<Muscle>(item.TargetMuscles),
            string.IsNullOrEmpty(item.GifUrl) ? null : new Uri(item.GifUrl)
        )).ToList();
    }

    public async Task<WorkoutExerciseDto> GetByIdAsync(string id)
    { 
        var requestUrl = $"exercises/{id}";
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new NotFoundException($"Exercise with id {id} not found.");
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<ExerciseDbSingleResponse>();

        if (apiResponse is null || !apiResponse.Success || apiResponse.Data is null)
        {
            throw new ServiceUnavailableException("Failed to retrieve exercise details.");
        }

        var item = apiResponse.Data;

        return new WorkoutExerciseDto(
            item.ExerciseId,
            item.Name,
            string.IsNullOrEmpty(item.GifUrl) ? null : new Uri(item.GifUrl),
            ParseEnums<Muscle>(item.TargetMuscles),
            ParseEnums<BodyPart>(item.BodyParts),
            ParseEnums<Equipment>(item.Equipments),
            ParseEnums<Muscle>(item.SecondaryMuscles),
            item.Instructions
        );
    }
    
    private static string EnumsToApiString<T>(List<T> enums) where T : Enum
    {
        var result = enums.Select(enumValue =>
            SplitRegex().Split(enumValue.ToString()))
            .Select(w => string.Join(" ", w.Select(s => s.ToLowerInvariant())))
            .ToList();

        return string.Join(",", result);
    }

    private static List<T> ParseEnums<T>(List<string>? values) where T : struct, Enum
    {
        if (values == null) return [];

        var result = new List<T>();
        foreach (var value in values)
        {
            var cleanValue = value.Replace(" ", "", StringComparison.OrdinalIgnoreCase);
            if (Enum.TryParse<T>(cleanValue, true, out var enumValue))
            {
                result.Add(enumValue);
            }
        }

        return result;
    }

    [GeneratedRegex("(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])")]
    private static partial Regex SplitRegex();
}