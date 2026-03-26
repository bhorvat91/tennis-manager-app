using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Matches;

[QueryProperty(nameof(ClubId), "ClubId")]
public partial class PlayerStatsViewModel : ObservableObject
{
    private readonly MatchService _matchService;

    [ObservableProperty]
    private Guid _clubId;

    [ObservableProperty]
    private ObservableCollection<PlayerStatsDto> _stats = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public PlayerStatsViewModel(MatchService matchService)
    {
        _matchService = matchService;
    }

    partial void OnClubIdChanged(Guid value)
    {
        if (value != Guid.Empty)
            LoadStatsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadStatsAsync()
    {
        if (ClubId == Guid.Empty) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var stats = await _matchService.GetPlayerStatsAsync(ClubId);
            Stats = new ObservableCollection<PlayerStatsDto>(stats.OrderByDescending(s => s.Wins));
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati statistiku.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
