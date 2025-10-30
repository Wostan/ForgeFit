using ForgeFit.Domain.Aggregates.WorkoutAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces.Repositories;

public interface IWorkoutExercisePlanRepository : IRepository<WorkoutExercisePlan>
{
    Task<List<WorkoutExercisePlan>> GetAllByWorkoutProgramIdAsync(Guid workoutProgramId);
}