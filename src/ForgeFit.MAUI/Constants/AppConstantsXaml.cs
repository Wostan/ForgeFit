using System.Globalization;

namespace ForgeFit.MAUI.Constants;

/// <summary>
///     Proxy class to expose nested AppConstants values for XAML binding.
///     XAML's x:Static cannot access nested types directly.
/// </summary>
public static class AppConstantsXaml
{
    // --- Authentication & Security ---
    public static int MaxUsernameLength => AppConstants.ValidationLimits.MaxUsernameLength;

    // --- User Profile ---
    public static double MinWeightKg => AppConstants.ValidationLimits.MinWeightKg;
    public static double MaxWeightKg => AppConstants.ValidationLimits.MaxWeightKg;

    public static string MinWeightKgStr =>
        AppConstants.ValidationLimits.MinWeightKg.ToString(CultureInfo.InvariantCulture);

    public static string MaxWeightKgStr =>
        AppConstants.ValidationLimits.MaxWeightKg.ToString(CultureInfo.InvariantCulture);

    public static double MinWeightLbs => AppConstants.ValidationLimits.MinWeightLbs;
    public static double MaxWeightLbs => AppConstants.ValidationLimits.MaxWeightLbs;
    public static double MinHeightCm => AppConstants.ValidationLimits.MinHeightCm;
    public static double MaxHeightCm => AppConstants.ValidationLimits.MaxHeightCm;

    public static string MinHeightCmStr =>
        AppConstants.ValidationLimits.MinHeightCm.ToString(CultureInfo.InvariantCulture);

    public static string MaxHeightCmStr =>
        AppConstants.ValidationLimits.MaxHeightCm.ToString(CultureInfo.InvariantCulture);

    public static double MinHeightInches => AppConstants.ValidationLimits.MinHeightInches;
    public static double MaxHeightInches => AppConstants.ValidationLimits.MaxHeightInches;

    // --- General Content ---
    public static int MaxTitleLength => AppConstants.ValidationLimits.MaxTitleLength;
    public static int MaxDescriptionLength => AppConstants.ValidationLimits.MaxDescriptionLength;

    // --- Workouts & Programs ---
    public static int MaxWorkoutProgramDescriptionLength =>
        AppConstants.ValidationLimits.MaxWorkoutProgramDescriptionLength;

    public static int MaxWorkoutDurationHours => AppConstants.ValidationLimits.MaxWorkoutDurationHours;

    // --- Exercises ---
    public static int MaxRepsPerSet => AppConstants.ValidationLimits.MaxRepsPerSet;
    public static double MaxWorkoutWeightKg => AppConstants.ValidationLimits.MaxWorkoutWeightKg;

    // --- Custom Food Input Limits ---
    public static int MaxFoodLabelLength => AppConstants.ValidationLimits.MaxFoodLabelLength;
    public static int MaxBarcodeLength => AppConstants.ValidationLimits.MaxBarcodeLength;
    public static int MacroEntryMaxLength => 4;

    // --- Recipe Input Limits ---
    public static int MaxFoodItemsPerMeal => AppConstants.ValidationLimits.MaxFoodItemsPerMeal;

    public static string MaxFoodItemsPerMealStr =>
        AppConstants.ValidationLimits.MaxFoodItemsPerMeal.ToString(CultureInfo.InvariantCulture);

    public static string MaxFoodLabelLengthStr =>
        AppConstants.ValidationLimits.MaxFoodLabelLength.ToString(CultureInfo.InvariantCulture);

    public static string MaxDescriptionLengthStr =>
        AppConstants.ValidationLimits.MaxDescriptionLength.ToString(CultureInfo.InvariantCulture);

    // --- Workout Input Limits ---
    public static int HoursEntryMaxLength => AppConstants.XamlInputLimits.HoursEntryMaxLength;
    public static int MinutesEntryMaxLength => AppConstants.XamlInputLimits.MinutesEntryMaxLength;
    public static int WeightEntryMaxLength => AppConstants.XamlInputLimits.WeightEntryMaxLength;
}
