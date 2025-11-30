using System.Security.Authentication;
using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class WorkoutTrackingService(
    IWorkoutEntryRepository workoutEntryRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IWorkoutTrackingService
{
    public async Task<WorkoutEntryDto> LogEntryAsync(Guid userId, WorkoutEntryDto workoutEntryDto)
    {
        if (!await userRepository.ExistsAsync(userId))
        {
            throw new NotFoundException("User not found");
        }
        
        var performedExercises = GetPerformedFromDto(workoutEntryDto);

        var schedule = new Schedule(workoutEntryDto.Start, workoutEntryDto.End);

        var entry = WorkoutEntry.Create(
            userId,
            workoutEntryDto.WorkoutProgramId,
            schedule,
            performedExercises
        );

        await workoutEntryRepository.AddAsync(entry);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<WorkoutEntryDto>(entry);
    }

    public async Task<WorkoutEntryDto> UpdateEntryAsync(Guid userId, Guid entryId, WorkoutEntryDto workoutEntryDto)
    { 
        var entry = await workoutEntryRepository.GetByIdAsync(entryId);
        
        if (entry == null)
        {
            throw new NotFoundException("Workout entry not found");
        }
        
        if (userId != entry.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this workout entry");
        }
        
        var schedule = new Schedule(workoutEntryDto.Start, workoutEntryDto.End);
        var performedExercises = GetPerformedFromDto(workoutEntryDto);

        entry.Update(schedule, performedExercises);

        await unitOfWork.SaveChangesAsync();

        return mapper.Map<WorkoutEntryDto>(entry);
    }

    public async Task DeleteEntryAsync(Guid userId, Guid entryId)
    { 
        var entry = await workoutEntryRepository.GetByIdAsync(entryId);
        
        if (entry == null)
        {
            throw new NotFoundException("Workout entry not found");
        }
        
        if (userId != entry.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this workout entry");
        }

        workoutEntryRepository.Remove(entry);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<WorkoutEntryDto> GetEntryAsync(Guid userId, Guid entryId)
    {
        var entry = await workoutEntryRepository.GetByIdWithNavigationsAsync(entryId);
        
        if (entry == null)
        {
            throw new NotFoundException("Workout entry not found");
        }
        
        if (userId != entry.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this workout entry");
        }

        return mapper.Map<WorkoutEntryDto>(entry);
    }

    public async Task<List<WorkoutEntryDto>> GetEntriesByDateAsync(Guid userId, DateTime date)
    {
        var entries = await workoutEntryRepository.GetAllByUserIdAndDateAsync(userId, date);
        return mapper.Map<List<WorkoutEntryDto>>(entries);
    }

    public async Task<List<WorkoutEntryDto>> GetEntriesByDateAsync(Guid userId, DateTime from, DateTime to)
    {
        var entries = await workoutEntryRepository.GetAllByUserIdAndDateRangeAsync(
            userId, 
            from, 
            to);
        return mapper.Map<List<WorkoutEntryDto>>(entries);
    }
    
    public static List<PerformedExercise> GetPerformedFromDto(WorkoutEntryDto workoutEntryDto)
    {
        return workoutEntryDto.PerformedExercises.Select(performedExerciseDto => 
        {
            var snapshot = new WorkoutExercise(
                performedExerciseDto.ExerciseSnapshot.ExternalId,
                performedExerciseDto.ExerciseSnapshot.Name,
                performedExerciseDto.ExerciseSnapshot.GifUrl,
                performedExerciseDto.ExerciseSnapshot.TargetMuscles,
                performedExerciseDto.ExerciseSnapshot.BodyParts,
                performedExerciseDto.ExerciseSnapshot.Equipment,
                performedExerciseDto.ExerciseSnapshot.SecondaryMuscles,
                performedExerciseDto.ExerciseSnapshot.Instructions
            );

            var sets = performedExerciseDto.Sets.Select(s => 
                new PerformedSet(
                    s.Order, 
                    s.Reps, 
                    new Weight(s.Weight, s.WeightUnit),
                    s.IsCompleted)
            ).ToList();

            return new PerformedExercise(snapshot, sets);
        }).ToList();
    }
}