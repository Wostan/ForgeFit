using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

public class FoodDiaryIntegrationViewModel(
    IDiaryService diaryService,
    IFoodService foodService,
    IAlertService alertService) : ObservableObject
{
    private DateTime _date;
    private Guid? _entryId;
    private DayTime _mealType;

    public Guid? EntryId
    {
        get => _entryId;
        set => SetProperty(ref _entryId, value);
    }

    public string EntryIdStr
    {
        get => _entryId?.ToString() ?? string.Empty;
        set
        {
            if (Guid.TryParse(value, out var id))
                _entryId = id;
        }
    }

    public HashSet<string> ExistingProductIds { get; private set; } = [];

    public Func<FoodSearchItemViewModel, Task>? QuickAddCallback { get; set; }
    public Func<FoodSearchItemViewModel, Task>? RemoveCallback { get; set; }

    public void Initialize(DateTime date, DayTime mealType, Guid? entryId)
    {
        _date = date;
        _mealType = mealType;
        _entryId = entryId;
        ExistingProductIds.Clear();
    }

    public async Task RefreshExistingIdsAsync()
    {
        try
        {
            if (!_entryId.HasValue) return;
            var result = await diaryService.GetEntryAsync(_entryId.Value);
            if (result is { Success: true, Data: not null })
                ExistingProductIds = result.Data.FoodItems.Select(x => x.ExternalId).ToHashSet();
        }
        catch
        {
            // ignored
        }
    }

    public async Task QuickAddInternal(FoodSearchItemViewModel itemVm, bool isShowingRecent)
    {
        try
        {
            var productResult = await foodService.GetProductByIdAsync(itemVm.Data.ExternalId);
            if (productResult is not { Success: true, Data: not null })
            {
                await alertService.ShowToastAsync(new LocalizedString(() => productResult.Message).Localized);
                return;
            }

            var product = productResult.Data;
            var parts = itemVm.Data.Serving.Split(' ');
            FoodServingDto? targetServing = null;
            double amount = 0;

            if (isShowingRecent && parts.Length >= 2 && double.TryParse(parts[0], out var historyAmount))
            {
                var historyUnit = parts.Last();
                targetServing = product.Servings.FirstOrDefault(s =>
                    string.Equals(s.MetricUnit, historyUnit, StringComparison.OrdinalIgnoreCase));
                if (targetServing != null)
                    amount = historyAmount;
            }

            if (targetServing == null)
            {
                targetServing = product.Servings.FirstOrDefault();
                amount = targetServing?.MetricAmount ?? 100;
            }

            var unit = targetServing?.MetricUnit ?? "g";
            var baseAmount = targetServing?.MetricAmount ?? 1;
            var ratio = amount / baseAmount;

            var newItem = new FoodItemDto(
                product.ExternalId,
                product.Label,
                (targetServing?.Calories ?? itemVm.Calories) * ratio,
                (targetServing?.Carbs ?? itemVm.Carbs) * ratio,
                (targetServing?.Protein ?? itemVm.Protein) * ratio,
                (targetServing?.Fat ?? itemVm.Fat) * ratio,
                unit,
                amount
            );

            await AddEntryToDiaryInternal(newItem);
            itemVm.IsAdded = true;
            ExistingProductIds.Add(product.ExternalId);
        }
        catch (Exception ex)
        {
            await alertService.ShowToastAsync(new LocalizedString(() => ex.Message).Localized);
        }
    }

    public async Task RemoveItemInternal(FoodSearchItemViewModel itemVm)
    {
        try
        {
            var entriesResult = await diaryService.GetEntriesByDateAsync(_date);
            if (entriesResult is not { Success: true, Data: not null }) return;

            var existingEntry = entriesResult.Data.FirstOrDefault(e => e.DayTime == _mealType);
            if (existingEntry == null) return;

            var itemToRemove = existingEntry.FoodItems.FirstOrDefault(x => x.ExternalId == itemVm.Data.ExternalId);
            if (itemToRemove == null) return;

            var updatedItems = existingEntry.FoodItems.ToList();
            updatedItems.Remove(itemToRemove);

            if (updatedItems.Count == 0)
            {
                await diaryService.DeleteEntryAsync(existingEntry.Id);
                _entryId = null;
            }
            else
            {
                var updateRequest = new FoodEntryCreateRequest(_mealType, _date, updatedItems);
                await diaryService.UpdateEntryAsync(existingEntry.Id, updateRequest);
            }

            WeakReferenceMessenger.Default.Send(new DiaryUpdatedMessage());
            itemVm.IsAdded = false;
            ExistingProductIds.Remove(itemVm.Data.ExternalId);
        }
        catch
        {
            await alertService.ShowToastAsync("Unexpected error occurred");
        }
    }

    public async Task AddEntryToDiaryInternal(FoodItemDto newItem)
    {
        if (_entryId.HasValue)
        {
            var entryResult = await diaryService.GetEntryAsync(_entryId.Value);
            if (entryResult is { Success: true, Data: not null })
            {
                var updatedItems = entryResult.Data.FoodItems.ToList();
                updatedItems.Add(newItem);
                var updateRequest = new FoodEntryCreateRequest(_mealType, _date, updatedItems);
                await diaryService.UpdateEntryAsync(_entryId.Value, updateRequest);
            }
        }
        else
        {
            var createRequest = new FoodEntryCreateRequest(_mealType, _date, [newItem]);
            var createResponse = await diaryService.CreateEntryAsync(createRequest);
            if (createResponse is { Success: true, Data: not null })
                _entryId = createResponse.Data.Id;
        }

        WeakReferenceMessenger.Default.Send(new DiaryUpdatedMessage());
    }

    public void ResetState()
    {
        ExistingProductIds.Clear();
        _entryId = null;
    }

    public bool IsProductAdded(string externalId)
    {
        return ExistingProductIds.Contains(externalId);
    }

    public void MarkProductAsAdded(string externalId)
    {
        ExistingProductIds.Add(externalId);
    }

    public void MarkProductAsRemoved(string externalId)
    {
        ExistingProductIds.Remove(externalId);
    }
}