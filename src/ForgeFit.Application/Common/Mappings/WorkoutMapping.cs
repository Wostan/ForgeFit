using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;
using Mapster;

namespace ForgeFit.Application.Common.Mappings;

public class WorkoutMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WorkoutEntry, WorkoutEntryDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.WorkoutProgramId, src => src.WorkoutProgramId)
            .Map(dest => dest.Start, src => src.WorkoutSchedule.Start)
            .Map(dest => dest.End, src => src.WorkoutSchedule.End)
            .Map(dest => dest.TotalReps, src => CalculateTotalReps(src.PerformedExercises))
            .Map(dest => dest.TotalVolume, src => CalculateTotalVolume(src.PerformedExercises))
            .Map(dest => dest.PerformedExercises, src => src.PerformedExercises);
        
        config.NewConfig<PerformedExercise, PerformedExerciseDto>()
            .Map(dest => dest.ExerciseSnapshot, src => src.Snapshot)
            .Map(dest => dest.Sets, src => src.Sets);
        
        config.NewConfig<PerformedSet, PerformedSetDto>()
            .Map(dest => dest.Order, src => src.Order)
            .Map(dest => dest.Reps, src => src.Reps)
            .Map(dest => dest.Weight, src => src.Weight.Value)
            .Map(dest => dest.WeightUnit, src => src.Weight.Unit)
            .Map(dest => dest.IsCompleted, src => src.IsCompleted);
    }
    
    private static double CalculateTotalReps(IEnumerable<PerformedExercise> exercises)
    {
        return exercises.Sum(e => e.Sets
            .Where(s => s.IsCompleted)
            .Sum(s => s.Reps));
    }

    private static double CalculateTotalVolume(IEnumerable<PerformedExercise> exercises)
    {
        return exercises.Sum(e => e.Sets
            .Where(s => s.IsCompleted)
            .Sum(s => s.Weight.ToKg().Value * s.Reps));
    }
}