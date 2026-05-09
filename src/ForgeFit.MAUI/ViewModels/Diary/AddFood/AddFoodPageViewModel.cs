using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;
using ForgeFit.MAUI.ViewModels.Diary.MyProducts;
using ForgeFit.MAUI.ViewModels.Diary.Recipes;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.AddFood;

public partial class AddFoodPageViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ILocalizationResourceManager _localizationManager;
    private bool _isInitialized;

    private DateTime _date;
    private DayTime _mealType;
    private Guid? _entryId;

    [ObservableProperty] private string _mealTitle = string.Empty;
    [ObservableProperty] private int _currentTabIndex;

    public AddFoodPageViewModel(
        IFoodService foodService,
        IDiaryService diaryService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager,
        ICustomFoodService customFoodService,
        IRecipeService recipeService)
    {
        _localizationManager = localizationManager;

        PopupVM = new PopupManagerViewModel(localizationManager);
        DiaryVM = new FoodDiaryIntegrationViewModel(diaryService, foodService, alertService, customFoodService);
        DetailsVM = new FoodDetailsViewModel(alertService);

        SearchVM = new FoodSearchViewModel(foodService, diaryService, alertService, localizationManager, DiaryVM, DetailsVM);
        CreateCustomFoodVM = new CreateCustomFoodViewModel(PopupVM, customFoodService, alertService, localizationManager);
        MyProductsVM = new MyProductsViewModel(PopupVM, customFoodService, alertService, localizationManager, DiaryVM, DetailsVM, CreateCustomFoodVM);
        CreateRecipeVM = new CreateRecipeViewModel(PopupVM, recipeService, foodService, customFoodService, alertService, localizationManager);
        RecipesVM = new RecipesViewModel(PopupVM, recipeService, alertService, localizationManager, DiaryVM, CreateRecipeVM);

        SetupCallbacks();
    }

    public FoodSearchViewModel SearchVM { get; }
    public FoodDetailsViewModel DetailsVM { get; }
    public FoodDiaryIntegrationViewModel DiaryVM { get; }
    public PopupManagerViewModel PopupVM { get; }
    public MyProductsViewModel MyProductsVM { get; }
    public CreateCustomFoodViewModel CreateCustomFoodVM { get; }
    public CreateRecipeViewModel CreateRecipeVM { get; }
    public RecipesViewModel RecipesVM { get; }

    [RelayCommand]
    private async Task OpenBarcodeScanner()
    {
        await Shell.Current.GoToAsync("BarcodeScannerPage");
    }

    [RelayCommand]
    private async Task OpenPhotoRecognition()
    {
        await Shell.Current.GoToAsync(
            $"PhotoRecognitionPage?Date={Uri.EscapeDataString(_date.ToString("O"))}&MealType={_mealType}&EntryId={_entryId?.ToString() ?? string.Empty}");
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!_isInitialized)
            ResetState();

        _isInitialized = true;
        IsLoading = true;

        if (query.TryGetValue("Date", out var dateObj) && DateTime.TryParse(dateObj.ToString(), out var date))
            _date = date;

        if (query.TryGetValue("MealType", out var typeObj) && Enum.TryParse<DayTime>(typeObj.ToString(), out var type))
        {
            _mealType = type;
            MealTitle = type switch
            {
                DayTime.Breakfast => _localizationManager["Meal_Breakfast"],
                DayTime.Lunch => _localizationManager["Meal_Lunch"],
                DayTime.Dinner => _localizationManager["Meal_Dinner"],
                _ => _localizationManager["Meal_Snack"]
            };
        }

        if (query.TryGetValue("EntryId", out var idObj) && Guid.TryParse(idObj.ToString(), out var id))
            _entryId = id;

        DiaryVM.Initialize(_date, _mealType, _entryId);
        await DiaryVM.RefreshExistingIdsAsync();
        
        await SearchVM.LoadRecentAsync();
        await MyProductsVM.LoadProductsAsync();
        await RecipesVM.LoadRecipesAsync();

        IsLoading = false;
    }

    private void SetupCallbacks()
    {
        DetailsVM.OpenFoodDetailsCallback = (product, source) =>
            DetailsVM.OpenFoodDetailsInternal(product, source, SearchVM.IsShowingRecent);
        DetailsVM.CloseFoodDetailsCallback = CloseFoodDetailsInternal;
        DetailsVM.SaveFoodCallback = SaveFoodInternal;

        WeakReferenceMessenger.Default.Register<BarcodeDetectedMessage>(this, async (recipient, message) =>
        {
            var barcode = message.Barcode;
            var p = message.Product;
            var baseServing = p.Servings.FirstOrDefault();

            var searchResponse = new FoodSearchResponse(
                p.ExternalId, p.Label, p.BrandName,
                baseServing?.Calories ?? 0, baseServing?.Carbs ?? 0,
                baseServing?.Protein ?? 0, baseServing?.Fat ?? 0,
                baseServing?.Fiber ?? 0, baseServing?.Sugar ?? 0,
                baseServing?.SaturatedFat ?? 0, baseServing?.Sodium ?? 0,
                $"{baseServing?.MetricAmount} {baseServing?.MetricUnit}");

            var itemVm = new FoodSearchItemViewModel(searchResponse);
            if (DiaryVM.IsProductAdded(p.ExternalId)) itemVm.IsAdded = true;

            SearchVM.ClearSearchResults();
            SearchVM.AddSearchResult(itemVm);
            await DetailsVM.OpenFoodDetailsInternal(p, itemVm.Data, false);
        });

        CreateCustomFoodVM.FoodCreatedCallback = MyProductsVM.OnFoodCreatedAsync;
        CreateCustomFoodVM.FoodUpdatedCallback = MyProductsVM.OnFoodUpdatedAsync;

        CreateRecipeVM.RecipeCreatedCallback = RecipesVM.OnRecipeCreatedAsync;
        CreateRecipeVM.RecipeUpdatedCallback = RecipesVM.OnRecipeUpdatedAsync;
    }

    private void ResetState()
    {
        SearchVM.ResetState();
        DetailsVM.ResetPopupState();
        DiaryVM.ResetState();
        RecipesVM.SearchText = string.Empty;
        CurrentTabIndex = 0;
        IsLoading = false;
    }

    [RelayCommand]
    private void ChangeTab(object parameter)
    {
        if (parameter is int i) CurrentTabIndex = i;
        else if (parameter is string s && int.TryParse(s, out var idx)) CurrentTabIndex = idx;
    }

    private Task CloseFoodDetailsInternal()
    {
        DetailsVM.IsFoodDetailsVisible = false;
        return Task.CompletedTask;
    }

    private async Task SaveFoodInternal(FoodItemDto newItem)
    {
        await DiaryVM.AddEntryToDiaryInternal(newItem);
    }

    [RelayCommand]
    private async Task Back()
    {
        if (DetailsVM.IsFoodDetailsVisible)
        {
            DetailsVM.IsFoodDetailsVisible = false;
            return;
        }

        if (DiaryVM.EntryId.HasValue)
            await Shell.Current.GoToAsync($"..?EntryId={Uri.EscapeDataString(DiaryVM.EntryId.Value.ToString())}", false);
        else
            await Shell.Current.GoToAsync("..", false);
    }
}