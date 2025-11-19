using ForgeFit.Domain.Aggregates.WorkoutAggregate;

namespace ForgeFit.Infrastructure.Interfaces.Repositories;

public interface IWorkoutExercisePlanRepository : IRepository<WorkoutExercisePlan>
{
    Task<List<WorkoutExercisePlan>> GetAllByWorkoutProgramIdAsync(Guid workoutProgramId);
}