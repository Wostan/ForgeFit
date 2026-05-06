using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;
using ForgeFit.MAUI.ViewModels.Diary.MyProducts;
using LocalizationResourceManager.Maui;

// using ForgeFit.MAUI.ViewModels.Diary.Recipes;

namespace ForgeFit.MAUI.ViewModels.Diary.AddFood;

public partial class AddFoodPageViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ILocalizationResourceManager _localizationManager;

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
        ICustomFoodService customFoodService)
    {
        _localizationManager = localizationManager;

        PopupVM = new PopupManagerViewModel(localizationManager);
        DiaryVM = new FoodDiaryIntegrationViewModel(diaryService, foodService, alertService, customFoodService);
        DetailsVM = new FoodDetailsViewModel(alertService);
        
        SearchVM = new FoodSearchViewModel(foodService, diaryService, alertService, localizationManager, DiaryVM, DetailsVM);
        ScannerVM = new FoodScannerViewModel(foodService, alertService, localizationManager, SearchVM, DetailsVM, DiaryVM);
        CreateCustomFoodVM = new CreateCustomFoodViewModel(PopupVM, customFoodService, alertService, localizationManager);
        MyProductsVM = new MyProductsViewModel(PopupVM, customFoodService, alertService, localizationManager, DiaryVM, DetailsVM, CreateCustomFoodVM);

        // TODO:
        // RecipesVM = new RecipesViewModel(...);

        SetupCallbacks();
    }

    public FoodSearchViewModel SearchVM { get; }
    public FoodDetailsViewModel DetailsVM { get; }
    public FoodScannerViewModel ScannerVM { get; }
    public FoodDiaryIntegrationViewModel DiaryVM { get; }
    public PopupManagerViewModel PopupVM { get; }
    public MyProductsViewModel MyProductsVM { get; }
    public CreateCustomFoodViewModel CreateCustomFoodVM { get; }
    
    // public RecipesViewModel RecipesVM { get; }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ResetState();
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
        
        IsLoading = false;
    }

    private void SetupCallbacks()
    {
        DetailsVM.OpenFoodDetailsCallback = (product, source) =>
            DetailsVM.OpenFoodDetailsInternal(product, source, SearchVM.IsShowingRecent);
        DetailsVM.CloseFoodDetailsCallback = CloseFoodDetailsInternal;
        DetailsVM.SaveFoodCallback = SaveFoodInternal;
        
        CreateCustomFoodVM.FoodCreatedCallback = MyProductsVM.OnFoodCreatedAsync;
        CreateCustomFoodVM.FoodUpdatedCallback = MyProductsVM.OnFoodUpdatedAsync;
    }

    private void ResetState()
    {
        SearchVM.ResetState();
        DetailsVM.ResetPopupState();
        ScannerVM.ResetState();
        DiaryVM.ResetState();
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

        if (ScannerVM.IsScannerVisible)
        {
            ScannerVM.IsScannerVisible = false;
            return;
        }

        if (DiaryVM.EntryId.HasValue)
            await Shell.Current.GoToAsync($"..?EntryId={Uri.EscapeDataString(DiaryVM.EntryId.Value.ToString())}", false);
        else
            await Shell.Current.GoToAsync("..", false);
    }
}