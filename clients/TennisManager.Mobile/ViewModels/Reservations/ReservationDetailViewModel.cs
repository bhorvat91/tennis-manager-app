using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Reservations;

[QueryProperty(nameof(Reservation), "Reservation")]
public partial class ReservationDetailViewModel : ObservableObject
{
    private readonly ReservationService _reservationService;

    [ObservableProperty]
    private ReservationDto? _reservation;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ReservationDetailViewModel(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [RelayCommand]
    private async Task CancelReservationAsync()
    {
        if (Reservation == null) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Otkazivanje", "Jeste li sigurni da želite otkazati ovu rezervaciju?", "Da", "Ne");
        if (!confirmed) return;

        IsLoading = true;
        try
        {
            await _reservationService.CancelReservationAsync(Guid.Empty, Reservation.Id);
            await Shell.Current.GoToAsync("//MyReservationsPage");
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
