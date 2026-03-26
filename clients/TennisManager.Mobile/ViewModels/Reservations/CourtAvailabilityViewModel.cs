using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Reservations;

[QueryProperty(nameof(Court), "Court")]
[QueryProperty(nameof(ClubId), "ClubId")]
public partial class CourtAvailabilityViewModel : ObservableObject
{
    private readonly CourtService _courtService;

    [ObservableProperty]
    private CourtDto? _court;

    [ObservableProperty]
    private Guid _clubId;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<AvailabilitySlot> _slots = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public CourtAvailabilityViewModel(CourtService courtService)
    {
        _courtService = courtService;
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        LoadSlotsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadSlotsAsync()
    {
        if (Court == null) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var slots = await _courtService.GetAvailabilityAsync(Court.Id, SelectedDate);
            Slots = new ObservableCollection<AvailabilitySlot>(slots);
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati dostupnost.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task BookSlotAsync(AvailabilitySlot slot)
    {
        if (slot == null || !slot.IsAvailable || Court == null) return;

        await Shell.Current.GoToAsync(nameof(Views.Reservations.CreateReservationPage),
            new Dictionary<string, object>
            {
                ["Court"] = Court,
                ["ClubId"] = ClubId,
                ["StartTime"] = slot.StartTime,
                ["EndTime"] = slot.EndTime
            });
    }
}
