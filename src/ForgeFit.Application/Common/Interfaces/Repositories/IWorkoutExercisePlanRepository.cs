using ForgeFit.Domain.Aggregates.WorkoutAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IWorkoutExercisePlanRepository : IRepository<WorkoutExercisePlan>
{
    Task<List<WorkoutExercisePlan>> GetAllByWorkoutProgramIdAsync(Guid workoutProgramId);
}