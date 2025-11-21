using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IWorkoutProgramRepository : IRepository<WorkoutProgram>
{
    Task<List<WorkoutProgram>> GetAllByUserIdAsync(Guid userId);
    Task<List<WorkoutProgram>> GetAllByUserIdAndWorkoutProgramNameAsync(Guid userId, string workoutProgramName);
}