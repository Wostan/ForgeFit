using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutSetDto(
    Guid Id,
    int Order,
    int Reps,
    Weight Weight);