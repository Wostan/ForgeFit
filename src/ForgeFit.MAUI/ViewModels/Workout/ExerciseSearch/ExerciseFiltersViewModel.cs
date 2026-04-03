using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.Enums.WorkoutEnums;

namespace ForgeFit.MAUI.ViewModels.Workout.ExerciseSearch;

public partial class ExerciseFiltersViewModel : ObservableObject
{
    [ObservableProperty] private bool _isFiltersVisible;

    public ObservableCollection<FilterItem<Muscle>> FilterMuscles { get; } = [];
    public ObservableCollection<FilterItem<BodyPart>> FilterBodyParts { get; } = [];
    public ObservableCollection<FilterItem<Equipment>> FilterEquipment { get; } = [];

    public Func<Task>? ApplyFiltersCallback { get; set; }

    public void InitializeFilters()
    {
        if (FilterMuscles.Count == 0)
            foreach (var muscle in Enum.GetValues<Muscle>())
                FilterMuscles.Add(new FilterItem<Muscle>(muscle, ConvertEnumToReadable(muscle.ToString())));

        if (FilterBodyParts.Count == 0)
            foreach (var part in Enum.GetValues<BodyPart>())
                FilterBodyParts.Add(new FilterItem<BodyPart>(part, ConvertEnumToReadable(part.ToString())));

        if (FilterEquipment.Count == 0)
            foreach (var eq in Enum.GetValues<Equipment>())
                FilterEquipment.Add(new FilterItem<Equipment>(eq, ConvertEnumToReadable(eq.ToString())));

        foreach (var item in FilterMuscles) item.IsSelected = false;
        foreach (var item in FilterBodyParts) item.IsSelected = false;
        foreach (var item in FilterEquipment) item.IsSelected = false;
    }

    private static string ConvertEnumToReadable(string text)
    {
        return MyRegex().Replace(text, " $1");
    }

    [RelayCommand]
    private void ToggleFilters()
    {
        IsFiltersVisible = !IsFiltersVisible;
    }

    [RelayCommand]
    private void ToggleFilterSelection(object? item)
    {
        switch (item)
        {
            case FilterItem<Muscle> muscleItem:
                muscleItem.IsSelected = !muscleItem.IsSelected;
                break;
            case FilterItem<BodyPart> bodyPartItem:
                bodyPartItem.IsSelected = !bodyPartItem.IsSelected;
                break;
            case FilterItem<Equipment> equipmentItem:
                equipmentItem.IsSelected = !equipmentItem.IsSelected;
                break;
        }
    }

    [RelayCommand]
    private async Task ApplyFilters()
    {
        IsFiltersVisible = false;
        if (ApplyFiltersCallback != null)
            await ApplyFiltersCallback();
    }

    public void ResetState()
    {
        IsFiltersVisible = false;
        FilterMuscles.Clear();
        FilterBodyParts.Clear();
        FilterEquipment.Clear();
    }

    public List<Muscle> GetSelectedMuscles()
    {
        return FilterMuscles.Where(x => x.IsSelected).Select(x => x.Value).ToList();
    }

    public List<BodyPart> GetSelectedBodyParts()
    {
        return FilterBodyParts.Where(x => x.IsSelected).Select(x => x.Value).ToList();
    }

    public List<Equipment> GetSelectedEquipment()
    {
        return FilterEquipment.Where(x => x.IsSelected).Select(x => x.Value).ToList();
    }

    [System.Text.RegularExpressions.GeneratedRegex("(\\B[A-Z])")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}