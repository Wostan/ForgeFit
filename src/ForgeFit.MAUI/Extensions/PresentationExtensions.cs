using ForgeFit.MAUI.ViewModels;
using ForgeFit.MAUI.Views;
using ForgeFit.MAUI.Views.Auth;
using ForgeFit.MAUI.Views.Diary;
using ForgeFit.MAUI.Views.Workout;
using ActiveWorkoutPageViewModel = ForgeFit.MAUI.ViewModels.Workout.ActiveSession.ActiveWorkoutPageViewModel;
using DiaryPageViewModel = ForgeFit.MAUI.ViewModels.Diary.Main.DiaryPageViewModel;
using ExerciseSearchPageViewModel = ForgeFit.MAUI.ViewModels.Workout.ExerciseSearch.ExerciseSearchPageViewModel;
using AddFoodPageViewModel = ForgeFit.MAUI.ViewModels.Diary.AddFood.AddFoodPageViewModel;
using LoginPageViewModel = ForgeFit.MAUI.ViewModels.Auth.LoginPageViewModel;
using MealDetailsPageViewModel = ForgeFit.MAUI.ViewModels.Diary.Meals.MealDetailsPageViewModel;
using ProfilePageView = ForgeFit.MAUI.Views.Profile.ProfilePageView;
using ProfilePageViewModel = ForgeFit.MAUI.ViewModels.Profile.Main.ProfilePageViewModel;
using RegistrationPageViewModel = ForgeFit.MAUI.ViewModels.Registration.RegistrationPageViewModel;
using WorkoutPageViewModel = ForgeFit.MAUI.ViewModels.Workout.Dashboard.WorkoutPageViewModel;
using WorkoutProgramEditorPageViewModel = ForgeFit.MAUI.ViewModels.Workout.ProgramEditor.WorkoutProgramEditorPageViewModel;

namespace ForgeFit.MAUI.Extensions;

public static class PresentationExtensions
{
    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<RegistrationPageViewModel>();
        builder.Services.AddTransient<LoginPageViewModel>();

        builder.Services.AddTransient<DiaryPageViewModel>();
        builder.Services.AddTransient<MealDetailsPageViewModel>();
        builder.Services.AddTransient<AddFoodPageViewModel>();

        builder.Services.AddTransient<WorkoutPageViewModel>();
        builder.Services.AddTransient<ActiveWorkoutPageViewModel>();
        builder.Services.AddTransient<ExerciseSearchPageViewModel>();
        builder.Services.AddTransient<WorkoutProgramEditorPageViewModel>();

        builder.Services.AddTransient<ProfilePageViewModel>();

        return builder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<RegistrationPageView>();
        builder.Services.AddTransient<LoginPageView>();

        builder.Services.AddTransient<DiaryPageView>();
        builder.Services.AddTransient<MealDetailsPageView>();
        builder.Services.AddTransient<AddFoodPageView>();

        builder.Services.AddTransient<WorkoutPageView>();
        builder.Services.AddTransient<ActiveWorkoutPageView>();
        builder.Services.AddTransient<ExerciseSearchPageView>();
        builder.Services.AddTransient<WorkoutProgramEditorPageView>();

        builder.Services.AddTransient<ProfilePageView>();

        return builder;
    }
}
