using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Reservations;

[QueryProperty(nameof(Court), "Court")]
[QueryProperty(nameof(ClubId), "ClubId")]
[QueryProperty(nameof(StartTime), "StartTime")]
[QueryProperty(nameof(EndTime), "EndTime")]
public partial class CreateReservationViewModel : ObservableObject
{
    private readonly ReservationService _reservationService;

    [ObservableProperty]
    private CourtDto? _court;

    [ObservableProperty]
    private Guid _clubId;

    [ObservableProperty]
    private DateTime _startTime;

    [ObservableProperty]
    private DateTime _endTime;

    [ObservableProperty]
    private string _type = "Training";

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public List<string> ReservationTypes { get; } = new() { "Training", "Match", "Tournament" };

    public CreateReservationViewModel(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [RelayCommand]
    private async Task CreateReservationAsync()
    {
        if (Court == null || ClubId == Guid.Empty)
        {
            ErrorMessage = "Nedostaju podaci o terenu ili klubu.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var request = new CreateReservationRequest
            {
                CourtId = Court.Id,
                StartTime = StartTime,
                EndTime = EndTime,
                Type = Type,
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes
            };

            await _reservationService.CreateReservationAsync(ClubId, request);
            await Shell.Current.GoToAsync("//MyReservationsPage");
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće kreirati rezervaciju.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
