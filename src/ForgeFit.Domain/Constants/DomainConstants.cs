namespace ForgeFit.Domain.Constants;

public static class DomainConstants
{
    #region Validation Limits
    public static class ValidationLimits
    {
        // --- Authentication & Security ---
        public const int MaxUsernameLength = 20;
        public const int MinPasswordLength = 6;
        public const int MaxEmailLength = 254;
        public const int MaxPasswordHashLength = 256;
        public const int MaxRefreshTokenLength = 512;
        
        // --- User Profile ---
        public const int MaxAvatarUrlLength = 2048;
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
        public const int MaxExerciseGifUrlLength = 500;
        public const int MaxExerciseInstructionsLength = 2000;
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
        
        // --- System ---
        public const int MaxExternalIdLength = 100;
    }
    #endregion
    
    #region Conversion Factors
    public static class ConversionFactors
    {
        public const double KgToLbs = 2.20462;
        public const double LbsToKg = 0.453592;
        public const double CmToInches = 0.393701;
        public const double InchesToCm = 2.54;
    }
    #endregion
    
    #region Time
    public static class Time
    {
        public const int MinutesPerHour = 60;
        public const int SecondsPerMinute = 60;
        public const int SecondsPerHour = 3600;
    }
    #endregion
}