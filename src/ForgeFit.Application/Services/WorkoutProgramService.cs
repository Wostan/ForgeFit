using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class WorkoutProgramService(
    IWorkoutProgramRepository workoutProgramRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IWorkoutProgramService
{
    public async Task<WorkoutProgramResponse> CreateWorkoutProgramAsync(
        Guid userId, 
        WorkoutProgramRequest workoutProgramRequest)
    {
        if (!await userRepository.ExistsAsync(userId))
            throw new NotFoundException("User not found");
        
        var program = WorkoutProgram.Create(
            userId,
            workoutProgramRequest.Name,
            workoutProgramRequest.Description,
            new List<WorkoutExercisePlan>()
        );
        
        foreach (var planDto in workoutProgramRequest.WorkoutExercisePlans)
        {
            var plan = CreateFromDto(program.Id, userId, planDto);
            program.AddWorkoutExercise(plan);
        }

        await workoutProgramRepository.AddAsync(program);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<WorkoutProgramResponse>(program);
    }

    public async Task<WorkoutProgramResponse> UpdateWorkoutProgramAsync(
        Guid userId, 
        Guid workoutProgramId, 
        WorkoutProgramRequest workoutProgramRequest)
    {
        var program = await workoutProgramRepository.GetByIdWithNavigationsAsync(workoutProgramId);
        
        if (program == null)
            throw new NotFoundException("Workout program not found");

        if (program.UserId != userId)
            throw new UnauthorizedAccessException("You do not own this program");
        
        var exercisesToRemove = program.WorkoutExercisePlans.ToList();
        foreach (var exercise in exercisesToRemove)
        {
            program.RemoveWorkoutExercise(exercise);
        }

        foreach (var planDto in workoutProgramRequest.WorkoutExercisePlans)
        {
            var plan = CreateFromDto(program.Id, userId, planDto);
            program.AddWorkoutExercise(plan);
        }
        
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<WorkoutProgramResponse>(program);
    }

    public async Task DeleteWorkoutProgramAsync(Guid userId, Guid workoutProgramId)
    {
        var program = await workoutProgramRepository.GetByIdAsync(workoutProgramId);
        
        if (program == null)
            throw new NotFoundException("Workout program not found");

        if (program.UserId != userId)
            throw new UnauthorizedAccessException("You do not own this program");
        
        program.SoftDelete();
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<List<WorkoutProgramResponse>> GetAllWorkoutProgramsAsync(Guid userId)
    {
        var programs = await workoutProgramRepository.GetAllByUserIdAsync(userId);
        return mapper.Map<List<WorkoutProgramResponse>>(programs);
    }

    public async Task<WorkoutProgramResponse> GetWorkoutProgramAsync(Guid userId, Guid workoutProgramId)
    {
        var program = await workoutProgramRepository.GetByIdWithNavigationsAsync(workoutProgramId);
        
        if (program == null)
            throw new NotFoundException("Workout program not found");
        
        return program.UserId != userId 
            ? throw new UnauthorizedAccessException("You do not own this program") 
            : mapper.Map<WorkoutProgramResponse>(program);
    }
    
    private static WorkoutExercisePlan CreateFromDto(Guid programId, Guid userId, WorkoutExercisePlanDto dto)
    {
        var exerciseVo = new WorkoutExercise(
            dto.WorkoutExercise.ExternalId,
            dto.WorkoutExercise.Name,
            dto.WorkoutExercise.GifUrl,
            dto.WorkoutExercise.TargetMuscles,
            dto.WorkoutExercise.BodyParts,
            dto.WorkoutExercise.Equipment,
            dto.WorkoutExercise.SecondaryMuscles,
            dto.WorkoutExercise.Instructions
        );

        var sets = dto.WorkoutSets.Select(sDto => WorkoutSet.Create(
            userId,
            sDto.Order,
            sDto.Reps,
            sDto.restTime,
            sDto.Weight
        )).ToList();

        return WorkoutExercisePlan.Create(programId, exerciseVo, sets);
    }
}