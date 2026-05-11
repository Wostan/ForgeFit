using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.PhotoRecognition;

public partial class PhotoRecognitionViewModel(
    IFoodService foodService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager,
    FoodDiaryIntegrationViewModel diaryVM)
    : ObservableObject, IQueryAttributable
{
    [ObservableProperty] private ImageSource? _capturedImageSource;
    [ObservableProperty] private bool _isCameraActive = true;

    [ObservableProperty] private bool _isCapturing;
    [ObservableProperty] private bool _isImageCaptured;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isSuccessState;
    [ObservableProperty] private bool _showRetakeButton;
    [ObservableProperty] private double _totalCalories;
    [ObservableProperty] private double _totalCarbs;
    [ObservableProperty] private double _totalFat;
    [ObservableProperty] private double _totalFiber;
    [ObservableProperty] private double _totalProtein;
    [ObservableProperty] private double _totalSaturatedFat;
    [ObservableProperty] private double _totalSodium;
    [ObservableProperty] private double _totalSugar;

    public ObservableCollection<FoodItemDto> RecognizedItems { get; } = [];

    public Func<CancellationToken, Task<Stream?>>? CaptureImageRequested { get; set; }
    public Action? RetakeRequested { get; set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        DateTime date = default;
        DayTime mealType = default;
        Guid? entryId = null;

        if (query.TryGetValue("Date", out var dateObj) &&
            DateTime.TryParse(
                dateObj.ToString(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var d))
            date = d;

        if (query.TryGetValue("MealType", out var typeObj) &&
            Enum.TryParse<DayTime>(typeObj.ToString(), out var t))
            mealType = t;

        if (query.TryGetValue("EntryId", out var idObj) &&
            Guid.TryParse(idObj.ToString(), out var id))
            entryId = id;

        Debug.WriteLine($"[PhotoRecognition] date={date}, meal={mealType}, entryId={entryId}");
        diaryVM.Initialize(date, mealType, entryId);
    }

    [RelayCommand]
    private async Task Close()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CapturePhotoAsync(CancellationToken cancellationToken)
    {
        if (CaptureImageRequested is null) return;

        IsCapturing = true;
        try
        {
            var stream = await CaptureImageRequested(cancellationToken);
            if (stream is null) return;

            await ProcessCapturedPhotoAsync(stream);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CapturePhoto OUTER] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsCapturing = false;
        }
    }

    private async Task ProcessCapturedPhotoAsync(Stream stream)
    {
        IsCameraActive = false;
        ShowRetakeButton = false;
        IsSuccessState = false;

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        CapturedImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        IsImageCaptured = true;

        IsLoading = true;

        try
        {
            var result = await Task.Run(async () =>
            {
                using var apiStream = new MemoryStream(imageBytes);
                return await foodService.RecognizeFoodFromImageAsync(apiStream);
            });

            if (result is { Success: true, Data: not null })
            {
                RecognizedItems.Clear();
                foreach (var product in result.Data)
                {
                    var serving = product.Servings.FirstOrDefault();
                    if (serving is null) continue;

                    var item = new FoodItemDto(
                        product.ExternalId, product.Label, serving.Calories, serving.Carbs,
                        serving.Protein, serving.Fat, serving.Fiber, serving.Sugar,
                        serving.SaturatedFat, serving.Sodium, serving.MetricUnit, serving.MetricAmount
                    );
                    RecognizedItems.Add(item);
                }

                CalculateTotals();

                IsSuccessState = true;
            }
            else
            {
                await alertService.ShowToastAsync(result.Message);
            }

            ShowRetakeButton = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CapturePhoto OUTER] {ex.GetType().Name}: {ex.Message}");
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
            ShowRetakeButton = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void RetakePhoto()
    {
        IsSuccessState = false;
        IsImageCaptured = false;
        ShowRetakeButton = false;
        CapturedImageSource = null;
        RecognizedItems.Clear();
        ResetTotals();
        IsLoading = false;
        RetakeRequested?.Invoke();
    }

    [RelayCommand]
    private async Task AddRecognizedItemsAsync()
    {
        if (RecognizedItems.Count == 0) return;

        await diaryVM.AddProductRangeAsync(RecognizedItems.ToList());
        await Shell.Current.GoToAsync("..");
    }

    private void ResetTotals()
    {
        TotalCalories = 0;
        TotalProtein = 0;
        TotalCarbs = 0;
        TotalFat = 0;
        TotalFiber = 0;
        TotalSugar = 0;
        TotalSaturatedFat = 0;
        TotalSodium = 0;
    }

    private void CalculateTotals()
    {
        TotalCalories = RecognizedItems.Sum(i => i.Calories);
        TotalProtein = RecognizedItems.Sum(i => i.Protein);
        TotalCarbs = RecognizedItems.Sum(i => i.Carbs);
        TotalFat = RecognizedItems.Sum(i => i.Fat);
        TotalFiber = RecognizedItems.Sum(i => i.Fiber);
        TotalSugar = RecognizedItems.Sum(i => i.Sugar);
        TotalSaturatedFat = RecognizedItems.Sum(i => i.SaturatedFat);
        TotalSodium = RecognizedItems.Sum(i => i.Sodium);
    }
}
