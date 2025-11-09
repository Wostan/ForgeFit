using ForgeFit.Domain.Aggregates.HabitAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces.Repositories;

public interface IHabitRepository : IRepository<Habit>
{
    Task<List<Habit>> GetAllByUserIdAsync(Guid userId);
    Task<List<Habit>> GetAllByUserIdAndHabitNameAsync(Guid userId, string habitName);
}