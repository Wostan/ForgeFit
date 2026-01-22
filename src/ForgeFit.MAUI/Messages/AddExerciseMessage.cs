using CommunityToolkit.Mvvm.Messaging.Messages;
using ForgeFit.MAUI.Models.DTOs.Workout;

namespace ForgeFit.MAUI.Messages;

public sealed class AddExerciseMessage(WorkoutExerciseDto value) : ValueChangedMessage<WorkoutExerciseDto>(value);
