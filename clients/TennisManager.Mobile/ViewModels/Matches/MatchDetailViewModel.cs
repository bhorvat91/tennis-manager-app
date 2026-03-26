using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Matches;

[QueryProperty(nameof(Match), "Match")]
[QueryProperty(nameof(ClubId), "ClubId")]
public partial class MatchDetailViewModel : ObservableObject
{
    private readonly MatchService _matchService;

    [ObservableProperty]
    private MatchDto? _match;

    [ObservableProperty]
    private Guid _clubId;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public MatchDetailViewModel(MatchService matchService)
    {
        _matchService = matchService;
    }

    [RelayCommand]
    private async Task EnterResultAsync()
    {
        if (Match == null) return;
        await Shell.Current.GoToAsync(nameof(Views.Matches.EnterResultPage),
            new Dictionary<string, object> { ["Match"] = Match, ["ClubId"] = ClubId });
    }
}
