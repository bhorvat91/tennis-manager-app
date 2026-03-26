using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Clubs;

[QueryProperty(nameof(Club), "Club")]
public partial class ClubDetailViewModel : ObservableObject
{
    private readonly ClubService _clubService;

    [ObservableProperty]
    private ClubDto? _club;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    public ClubDetailViewModel(ClubService clubService)
    {
        _clubService = clubService;
    }

    [RelayCommand]
    private async Task RequestMembershipAsync()
    {
        if (Club == null) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            await _clubService.RequestMembershipAsync(Club.Id, new RequestMembershipRequest());
            SuccessMessage = "Zahtjev za članstvo je poslan!";
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće poslati zahtjev za članstvo.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ViewCourtsAsync()
    {
        if (Club == null) return;
        await Shell.Current.GoToAsync(nameof(Views.Reservations.CourtListPage),
            new Dictionary<string, object> { ["ClubId"] = Club.Id });
    }
}
