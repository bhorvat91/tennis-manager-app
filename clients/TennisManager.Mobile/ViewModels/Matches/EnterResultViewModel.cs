using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Matches;

[QueryProperty(nameof(Match), "Match")]
[QueryProperty(nameof(ClubId), "ClubId")]
public partial class EnterResultViewModel : ObservableObject
{
    private readonly MatchService _matchService;

    [ObservableProperty]
    private MatchDto? _match;

    [ObservableProperty]
    private Guid _clubId;

    [ObservableProperty]
    private ObservableCollection<MatchSetDto> _sets = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public EnterResultViewModel(MatchService matchService)
    {
        _matchService = matchService;
        Sets.Add(new MatchSetDto { SetNumber = 1 });
    }

    [RelayCommand]
    private void AddSet()
    {
        if (Sets.Count < 5)
            Sets.Add(new MatchSetDto { SetNumber = Sets.Count + 1 });
    }

    [RelayCommand]
    private void RemoveLastSet()
    {
        if (Sets.Count > 1)
            Sets.RemoveAt(Sets.Count - 1);
    }

    [RelayCommand]
    private async Task SubmitResultAsync()
    {
        if (Match == null) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var request = new EnterResultRequest
            {
                WinnerId = Guid.Empty, // User selects winner
                Sets = Sets.ToList()
            };
            await _matchService.EnterResultAsync(ClubId, Match.Id, request);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće unijeti rezultat.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
