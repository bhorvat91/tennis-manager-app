using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Matches;

[QueryProperty(nameof(ClubId), "ClubId")]
public partial class MatchListViewModel : ObservableObject
{
    private readonly MatchService _matchService;

    [ObservableProperty]
    private Guid _clubId;

    [ObservableProperty]
    private ObservableCollection<MatchDto> _matches = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public MatchListViewModel(MatchService matchService)
    {
        _matchService = matchService;
    }

    partial void OnClubIdChanged(Guid value)
    {
        if (value != Guid.Empty)
            LoadMatchesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadMatchesAsync()
    {
        if (ClubId == Guid.Empty) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var matches = await _matchService.GetMatchesAsync(ClubId);
            Matches = new ObservableCollection<MatchDto>(matches.OrderByDescending(m => m.ScheduledAt));
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati mečeve.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToMatchAsync(MatchDto match)
    {
        await Shell.Current.GoToAsync(nameof(Views.Matches.MatchDetailPage),
            new Dictionary<string, object> { ["Match"] = match, ["ClubId"] = ClubId });
    }

    [RelayCommand]
    private async Task CreateMatchAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.Matches.CreateMatchPage),
            new Dictionary<string, object> { ["ClubId"] = ClubId });
    }
}
