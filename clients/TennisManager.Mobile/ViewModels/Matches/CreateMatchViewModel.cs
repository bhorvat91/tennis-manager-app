using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Matches;

[QueryProperty(nameof(ClubId), "ClubId")]
public partial class CreateMatchViewModel : ObservableObject
{
    private readonly MatchService _matchService;
    private readonly MemberService _memberService;

    [ObservableProperty]
    private Guid _clubId;

    [ObservableProperty]
    private string _type = "Singles";

    [ObservableProperty]
    private DateTime _scheduledAt = DateTime.Now.AddHours(1);

    [ObservableProperty]
    private ObservableCollection<ClubMemberDto> _members = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public List<string> MatchTypes { get; } = new() { "Singles", "Doubles" };

    public CreateMatchViewModel(MatchService matchService, MemberService memberService)
    {
        _matchService = matchService;
        _memberService = memberService;
    }

    partial void OnClubIdChanged(Guid value)
    {
        if (value != Guid.Empty)
            LoadMembersCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadMembersAsync()
    {
        if (ClubId == Guid.Empty) return;
        try
        {
            var members = await _memberService.GetMembersAsync(ClubId);
            Members = new ObservableCollection<ClubMemberDto>(members);
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati članove.";
        }
    }

    [RelayCommand]
    private async Task CreateMatchAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var request = new CreateMatchRequest
            {
                Type = Type,
                ScheduledAt = ScheduledAt
            };
            await _matchService.CreateMatchAsync(ClubId, request);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće kreirati meč.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
