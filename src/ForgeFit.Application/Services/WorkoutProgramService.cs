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
        if (!await userRepository.ExistsAsync(userId)) throw new NotFoundException("User not found");

        var program = WorkoutProgram.Create(
            userId,
            workoutProgramRequest.Name,
            workoutProgramRequest.Description,
            new List<WorkoutExercisePlan>()
        );

        var exercises = workoutProgramRequest.WorkoutExercisePlans
            .Select(planDto => CreateExercisePlanEntity(program.Id, userId, planDto))
            .ToList();

        foreach (var exercise in exercises) program.AddExercisePlan(exercise);

        await workoutProgramRepository.AddAsync(program);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<WorkoutProgramResponse>(program);
    }

    public async Task<WorkoutProgramResponse> UpdateWorkoutProgramAsync(
        Guid userId,
        Guid workoutProgramId,
        WorkoutProgramRequest request)
    {
        var program = await workoutProgramRepository.GetByIdWithNavigationsAsync(workoutProgramId);

        if (program == null) throw new NotFoundException("Workout program not found");
        if (program.UserId != userId) throw new UnauthorizedAccessException("You do not own this program");

        program.UpdateDetails(request.Name, request.Description);

        var existingPlans = program.WorkoutExercisePlans.ToList();
        var incomingPlanIds = request.WorkoutExercisePlans
            .Where(p => p.Id != Guid.Empty)
            .Select(p => p.Id)
            .ToList();

        var plansToRemove = existingPlans.Where(p => !incomingPlanIds.Contains(p.Id)).ToList();
        foreach (var plan in plansToRemove) program.RemoveExercisePlan(plan);

        foreach (var existingPlan in existingPlans.Where(p => !plansToRemove.Contains(p)))
        {
            var incomingDto = request.WorkoutExercisePlans.FirstOrDefault(p => p.Id == existingPlan.Id);
            if (incomingDto == null) continue;

            var existingSets = existingPlan.WorkoutSets.ToList();
            var incomingSetIds = incomingDto.WorkoutSets
                .Where(s => s.Id != Guid.Empty)
                .Select(s => s.Id)
                .ToList();

            var setsToRemove = existingSets.Where(s => !incomingSetIds.Contains(s.Id)).ToList();
            foreach (var set in setsToRemove) existingPlan.RemoveSet(set);
        }

        foreach (var dto in request.WorkoutExercisePlans)
        {
            var existingPlan = program.WorkoutExercisePlans.FirstOrDefault(p => p.Id == dto.Id && p.Id != Guid.Empty);

            if (existingPlan == null)
            {
                var newPlan = CreateExercisePlanEntity(program.Id, userId, dto);

                await workoutProgramRepository.AddPlanAsync(newPlan);

                program.AddExercisePlan(newPlan);
            }
            else
            {
                var exerciseVo = MapToWorkoutExerciseVo(dto.WorkoutExercise);
                existingPlan.UpdateWorkoutExercise(exerciseVo);

                foreach (var setDto in dto.WorkoutSets)
                {
                    var existingSet = existingPlan.WorkoutSets
                        .FirstOrDefault(s => s.Id == setDto.Id && s.Id != Guid.Empty);

                    if (existingSet == null)
                    {
                        var newSet = WorkoutSet.Create(
                            userId,
                            existingPlan.Id,
                            setDto.Order,
                            setDto.Reps,
                            setDto.RestTime,
                            new Weight(setDto.Weight, setDto.WeightUnit)
                        );

                        existingPlan.AddSet(newSet);
                    }
                    else
                    {
                        existingSet.Update(
                            setDto.Order,
                            setDto.Reps,
                            setDto.RestTime,
                            new Weight(setDto.Weight, setDto.WeightUnit)
                        );
                    }
                }
            }
        }

        await unitOfWork.SaveChangesAsync();

        return mapper.Map<WorkoutProgramResponse>(program);
    }

    public async Task DeleteWorkoutProgramAsync(Guid userId, Guid workoutProgramId)
    {
        var program = await workoutProgramRepository.GetByIdAsync(workoutProgramId);

        if (program == null) throw new NotFoundException("Workout program not found");
        if (program.UserId != userId) throw new UnauthorizedAccessException("You do not own this program");

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

        if (program == null) throw new NotFoundException("Workout program not found");
        if (program.UserId != userId) throw new UnauthorizedAccessException("You do not own this program");

        return mapper.Map<WorkoutProgramResponse>(program);
    }

    private static WorkoutExercisePlan CreateExercisePlanEntity(Guid programId, Guid userId, WorkoutExercisePlanDto dto)
    {
        var exerciseVo = MapToWorkoutExerciseVo(dto.WorkoutExercise);
        var plan = WorkoutExercisePlan.Create(programId, exerciseVo, new List<WorkoutSet>());

        foreach (var sDto in dto.WorkoutSets)
        {
            var set = WorkoutSet.Create(
                userId,
                plan.Id,
                sDto.Order,
                sDto.Reps,
                sDto.RestTime,
                new Weight(sDto.Weight, sDto.WeightUnit)
            );
            plan.AddSet(set);
        }

        return plan;
    }

    private static WorkoutExercise MapToWorkoutExerciseVo(WorkoutExerciseDto dto)
    {
        return new WorkoutExercise(
            dto.ExternalId,
            dto.Name,
            dto.GifUrl is null ? null : new Uri(dto.GifUrl),
            dto.TargetMuscles,
            dto.BodyParts,
            dto.Equipment,
            dto.SecondaryMuscles,
            dto.Instructions
        );
    }
}
