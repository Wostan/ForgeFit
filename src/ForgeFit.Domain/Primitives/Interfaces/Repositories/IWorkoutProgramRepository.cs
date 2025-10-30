using ForgeFit.Domain.Aggregates.WorkoutAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces.Repositories;

public interface IWorkoutProgramRepository : IRepository<WorkoutProgram>
{
    Task<List<WorkoutProgram>> GetAllByUserIdAsync(Guid userId);
    Task<List<WorkoutProgram>> GetAllByUserIdAndWorkoutProgramNameAsync(Guid userId, string workoutProgramName);
}