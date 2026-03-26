using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Reservations;

public partial class MyReservationsViewModel : ObservableObject
{
    private readonly ReservationService _reservationService;
    private readonly ClubService _clubService;

    [ObservableProperty]
    private ObservableCollection<ReservationDto> _upcomingReservations = new();

    [ObservableProperty]
    private ObservableCollection<ReservationDto> _pastReservations = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public MyReservationsViewModel(ReservationService reservationService, ClubService clubService)
    {
        _reservationService = reservationService;
        _clubService = clubService;
    }

    [RelayCommand]
    private async Task LoadReservationsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var all = await _reservationService.GetMyReservationsAsync();
            var now = DateTime.UtcNow;
            UpcomingReservations = new ObservableCollection<ReservationDto>(
                all.Where(r => r.StartTime >= now && r.Status != "Cancelled").OrderBy(r => r.StartTime));
            PastReservations = new ObservableCollection<ReservationDto>(
                all.Where(r => r.StartTime < now || r.Status == "Cancelled").OrderByDescending(r => r.StartTime));
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati rezervacije.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(ReservationDto reservation)
    {
        await Shell.Current.GoToAsync(nameof(Views.Reservations.ReservationDetailPage),
            new Dictionary<string, object> { ["Reservation"] = reservation });
    }

    [RelayCommand]
    private async Task CancelReservationAsync(ReservationDto reservation)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Otkazivanje", "Jeste li sigurni da želite otkazati ovu rezervaciju?", "Da", "Ne");
        if (!confirmed) return;

        IsLoading = true;
        try
        {
            // Use a placeholder clubId from the reservation context
            await _reservationService.CancelReservationAsync(Guid.Empty, reservation.Id);
            await LoadReservationsAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće otkazati rezervaciju.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
