using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Matches;

public partial class MyMatchesViewModel : ObservableObject
{
    private readonly MatchService _matchService;

    [ObservableProperty]
    private ObservableCollection<MatchDto> _upcomingMatches = new();

    [ObservableProperty]
    private ObservableCollection<MatchDto> _pastMatches = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public MyMatchesViewModel(MatchService matchService)
    {
        _matchService = matchService;
    }

    [RelayCommand]
    private async Task LoadMyMatchesAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var all = await _matchService.GetMyMatchesAsync();
            var now = DateTime.UtcNow;
            UpcomingMatches = new ObservableCollection<MatchDto>(
                all.Where(m => m.ScheduledAt >= now).OrderBy(m => m.ScheduledAt));
            PastMatches = new ObservableCollection<MatchDto>(
                all.Where(m => m.ScheduledAt < now).OrderByDescending(m => m.ScheduledAt));
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
            new Dictionary<string, object> { ["Match"] = match });
    }
}
