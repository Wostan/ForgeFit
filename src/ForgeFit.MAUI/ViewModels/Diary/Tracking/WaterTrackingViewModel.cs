using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.DrinkTracking;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.Tracking;

public partial class WaterTrackingViewModel : BaseViewModel
{
    private readonly IAlertService _alertService;
    private readonly IDrinkTrackingService _drinkTrackingService;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WaterProgress))]
    private double _currentWater;

    [ObservableProperty] private bool _isWaterInputVisible;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WaterProgress))]
    private double _targetWater;

    [ObservableProperty] private string? _targetWaterDisplay;

    [ObservableProperty] private ObservableCollection<DrinkEntryResponse> _waterEntries = [];

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveCustomWaterCommand))]
    private string _waterInputValue = "";

    public WaterTrackingViewModel(
        IDrinkTrackingService drinkTrackingService,
        IAlertService alertService)
    {
        _drinkTrackingService = drinkTrackingService;
        _alertService = alertService;
        SetLoadingState();
    }

    public double WaterProgress => TargetWater > 0 ? CurrentWater / TargetWater : 0;

    private void SetLoadingState()
    {
        TargetWaterDisplay = "-";
    }

    public async Task LoadWaterEntriesAsync(DateTime selectedDate, CancellationToken token = default)
    {
        try
        {
            var result = await _drinkTrackingService.GetEntriesByDateAsync(selectedDate, token);
            if (token.IsCancellationRequested) return;

            if (result is { Success: true, Data: not null })
            {
                var sortedData = result.Data.OrderByDescending(e => e.Date);
                WaterEntries = new ObservableCollection<DrinkEntryResponse>(sortedData);
                CurrentWater = WaterEntries.Sum(e => e.VolumeMl);
                OnPropertyChanged(nameof(WaterProgress));
            }
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
            {
                SetLoadingState();
                ResetWater();
            }
        }
    }

    public void SetWaterGoal(double waterGoal)
    {
        TargetWater = waterGoal;
        TargetWaterDisplay = TargetWater.ToString("F0");
        OnPropertyChanged(nameof(WaterProgress));
    }

    [RelayCommand]
    private async Task AddWater(int volumeMl)
    {
        if (volumeMl <= 0) return;

        var timestamp = DateTime.Now;

        var tempEntry = new DrinkEntryResponse(Guid.Empty, volumeMl, timestamp);

        WaterEntries.Insert(0, tempEntry);
        CurrentWater += volumeMl;
        OnPropertyChanged(nameof(WaterProgress));

        var request = new DrinkEntryCreateRequest(volumeMl, timestamp);
        var result = await _drinkTrackingService.CreateEntryAsync(request);

        if (result is { Success: true, Data: not null })
        {
            var index = WaterEntries.IndexOf(tempEntry);
            if (index != -1) WaterEntries[index] = result.Data;
            WeakReferenceMessenger.Default.Send(new WaterDataChangedMessage());
        }
        else
        {
            WaterEntries.Remove(tempEntry);
            CurrentWater -= volumeMl;
            OnPropertyChanged(nameof(WaterProgress));

            var errorMsg = new LocalizedString(() => result.Message);
            await _alertService.ShowToastAsync(errorMsg.Localized);
        }
    }

    [RelayCommand]
    private async Task DeleteWaterEntry(DrinkEntryResponse entry)
    {
        var index = WaterEntries.IndexOf(entry);
        if (index == -1) return;

        WaterEntries.Remove(entry);
        CurrentWater -= entry.VolumeMl;
        OnPropertyChanged(nameof(WaterProgress));

        var result = await _drinkTrackingService.DeleteEntryAsync(entry.Id);

        if (!result.Success)
        {
            if (index <= WaterEntries.Count)
                WaterEntries.Insert(index, entry);
            else
                WaterEntries.Add(entry);

            CurrentWater += entry.VolumeMl;
            OnPropertyChanged(nameof(WaterProgress));

            var errorMsg = new LocalizedString(() => result.Message);
            await _alertService.ShowToastAsync(errorMsg.Localized);
        }
        else
        {
            WeakReferenceMessenger.Default.Send(new WaterDataChangedMessage());
        }
    }

    [RelayCommand]
    private void OpenWaterInput()
    {
        WaterInputValue = AppConstants.DefaultValues.DefaultWaterAmountMl.ToString();
        IsWaterInputVisible = true;
    }

    [RelayCommand]
    private void CloseWaterInput()
    {
        IsWaterInputVisible = false;
    }

    [RelayCommand(CanExecute = nameof(CanSaveCustomWater))]
    private async Task SaveCustomWater()
    {
        if (int.TryParse((string?)WaterInputValue, out var amount) && amount > 0)
        {
            await AddWater(amount);
            CloseWaterInput();
        }
    }

    private bool CanSaveCustomWater()
    {
        return int.TryParse((string?)WaterInputValue, out var amount) &&
               amount is > AppConstants.ValidationLimits.MinDrinkVolumeMl
                   and < AppConstants.ValidationLimits.MaxDrinkVolumeMl;
    }

    public void ResetWater()
    {
        CurrentWater = 0;
        WaterEntries.Clear();
        OnPropertyChanged(nameof(WaterProgress));
    }
}
