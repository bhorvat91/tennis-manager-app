using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Clubs;

public partial class MyClubsViewModel : ObservableObject
{
    private readonly ClubService _clubService;

    [ObservableProperty]
    private ObservableCollection<ClubDto> _clubs = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public MyClubsViewModel(ClubService clubService)
    {
        _clubService = clubService;
    }

    [RelayCommand]
    private async Task LoadMyClubsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var clubs = await _clubService.GetMyClubsAsync();
            Clubs = new ObservableCollection<ClubDto>(clubs);
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati vaše klubove.";
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

    [RelayCommand]
    private async Task BrowseClubsAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.Clubs.ClubListPage));
    }
}
