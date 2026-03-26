using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Reservations;

[QueryProperty(nameof(ClubId), "ClubId")]
public partial class CourtListViewModel : ObservableObject
{
    private readonly CourtService _courtService;

    [ObservableProperty]
    private Guid _clubId;

    [ObservableProperty]
    private ObservableCollection<CourtDto> _courts = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public CourtListViewModel(CourtService courtService)
    {
        _courtService = courtService;
    }

    partial void OnClubIdChanged(Guid value)
    {
        if (value != Guid.Empty)
            LoadCourtsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadCourtsAsync()
    {
        if (ClubId == Guid.Empty) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var courts = await _courtService.GetCourtsAsync(ClubId);
            Courts = new ObservableCollection<CourtDto>(courts.Where(c => c.IsActive));
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati terene.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToAvailabilityAsync(CourtDto court)
    {
        await Shell.Current.GoToAsync(nameof(Views.Reservations.CourtAvailabilityPage),
            new Dictionary<string, object> { ["Court"] = court, ["ClubId"] = ClubId });
    }
}
