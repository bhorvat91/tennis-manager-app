using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Clubs;

public partial class ClubListViewModel : ObservableObject
{
    private readonly ClubService _clubService;

    [ObservableProperty]
    private ObservableCollection<ClubDto> _clubs = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ClubListViewModel(ClubService clubService)
    {
        _clubService = clubService;
    }

    [RelayCommand]
    private async Task LoadClubsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var clubs = await _clubService.GetClubsAsync();
            Clubs = new ObservableCollection<ClubDto>(clubs);
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati klubove.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToClubAsync(ClubDto club)
    {
        await Shell.Current.GoToAsync(nameof(Views.Clubs.ClubDetailPage),
            new Dictionary<string, object> { ["Club"] = club });
    }
}
