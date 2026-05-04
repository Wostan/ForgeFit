namespace ForgeFit.MAUI.Constants;

public static class AppConstants
{
    #region Validation Limits
    public static class ValidationLimits
    {
        // --- Authentication & Security ---
        public const int MaxUsernameLength = 20;
        public const int MinPasswordLength = 6;
        public const int MaxEmailLength = 254;

        // --- User Profile ---
        public const int MinAgeYears = 13;
        public const int MaxAgeYears = 100;
        public const double MinWeightKg = 30;
        public const double MaxWeightKg = 300;
        public const double MinWeightLbs = 66;
        public const double MaxWeightLbs = 661;
        public const double MinHeightCm = 100;
        public const double MaxHeightCm = 250;
        public const double MinHeightInches = 39;
        public const double MaxHeightInches = 98;

        // --- General Content ---
        public const int MaxTitleLength = 100;
        public const int MaxDescriptionLength = 500;

        // --- Workouts & Programs ---
        public const int MaxWorkoutProgramNameLength = 50;
        public const int MaxWorkoutProgramDescriptionLength = 300;
        public const int MinWorkoutsPerWeek = 1;
        public const int MaxWorkoutsPerWeek = 7;
        public const int MinWorkoutDurationMinutes = 5;
        public const int MaxWorkoutDurationHours = 5;

        // --- Exercises ---
        public const int MaxExerciseNameLength = 50;
        public const int MaxExercisesPerProgram = 50;
        public const int MaxWorkoutProgramsPerUser = 20;
        public const int MaxSetsPerExercise = 20;
        public const int MaxRepsPerSet = 100;
        public const int MaxRestTimeMinutes = 10;
        public const double MaxWorkoutWeightKg = 1500;

        // --- Nutrition & Goals ---
        public const int MinDailyCalories = 1200;
        public const int MaxDailyCalories = 10000;
        public const int MinWaterIntakeMl = 1000;
        public const int MaxWaterIntakeMl = 10000;

        // --- Food & Drinks ---
        public const int MaxFoodLabelLength = 100;
        public const int MaxBarcodeLength = 100;
        public const int MaxServingUnitLength = 20;
        public const double MinFoodAmount = 1;
        public const double MaxFoodAmount = 5000;
        public const int MaxFoodItemsPerMeal = 50;
        public const int MinDrinkVolumeMl = 50;
        public const int MaxDrinkVolumeMl = 2000;
        public const int MaxRecipesPerUser = 20;
    }
    #endregion

    #region Conversion Factors
    public static class ConversionFactors
    {
        public const double KgToLbs = 2.20462;
        public const double LbsToKg = 0.453592;
        public const double CmToInches = 0.393701;
        public const double InchesToCm = 2.54;
        public const double CmToM = 0.01;
    }
    #endregion

    #region Time
    public static class Time
    {
        public const int MinutesPerHour = 60;
        public const int SecondsPerMinute = 60;
        public const int SecondsPerHour = 3600;
        public const double DefaultRestTimeMinutes = 1.5;
    }
    #endregion

    #region BMI Thresholds (WHO Standards)
    public static class BmiThresholds
    {
        public const double UnderweightMax = 18.5;
        public const double NormalMax = 25.0;
        public const double OverweightMax = 30.0;
    }
    #endregion

    #region Goal Validation
    public static class GoalValidation
    {
        public const double MaxFatLossKgPerWeek = 1.3;
        public const double MaxMuscleGainKgPerWeek = 0.6;
        public const double MaxWeightGainKgPerWeek = 1.5;
        public const int MinDaysToDeadline = 7;
        public const int DefaultGoalMonthsAhead = 3;
    }
    #endregion

    #region Default Values
    public static class DefaultValues
    {
        public const int DefaultReps = 10;
        public const int DefaultWaterAmountMl = 250;
        public const double DefaultWeightKg = 80.0;
        public const double DefaultHeightCm = 175.0;
        public const double DefaultWeightChangeStep = 0.1;
    }
    #endregion

    #region Formatting Precision
    public static class FormattingPrecision
    {
        public const double DoubleComparisonEpsilon = 0.01;
        public const double NumericClampEpsilon = 0.001;
        public const int WeightDecimalPlaces = 1;
        public const int BmiDecimalPlaces = 1;
    }
    #endregion

    #region XAML Input Limits
    public static class XamlInputLimits
    {
        public const int HoursEntryMaxLength = 1;      // For hours input (0-5)
        public const int MinutesEntryMaxLength = 2;      // For minutes input (0-59)
        public const int WeightEntryMaxLength = 5;     // For weight input ("150.5")
    }
    #endregion

    #region Search & Debounce
    public static class SearchConfig
    {
        public const int DebounceDelayMs = 800;
        public const int DefaultPageSize = 20;
    }
    #endregion

    #region Food & Nutrition Defaults
    public static class FoodDefaults
    {
        public const int DefaultServingAmount = 100;
        public const int RecentItemsLookupDays = 7;
    }
    #endregion

    #region Meal Calorie Distribution (WHO/standard recommendations)
    public static class MealRatios
    {
        public const double Breakfast = 0.25;
        public const double Lunch = 0.35;
        public const double Dinner = 0.25;
        public const double Snack = 0.15;
    }
    #endregion

    #region Macro Nutrient Ratios (standard balanced diet)
    public static class MacroRatios
    {
        public const double Carbs = 0.5;
        public const double Protein = 0.2;
        public const double Fat = 0.3;
    }
    #endregion

    #region Calorie Conversion (per gram)
    public static class CaloriePerGram
    {
        public const int Carbs = 4;
        public const int Protein = 4;
        public const int Fat = 9;
    }
    #endregion

    #region Micronutrient Factors
    public static class MicronutrientFactors
    {
        public const double FiberGramsPer1000Kcal = 14.0;
        public const double SugarCalorieRatio = 0.10;
        public const double SaturatedFatCalorieRatio = 0.10;
        public const int SodiumLimitMg = 2300;
    }
    #endregion
}
