using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Profile;

public partial class ProfileViewModel : ObservableObject
{
    private readonly UserService _userService;

    [ObservableProperty]
    private UserProfileDto? _profile;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _phone = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    [ObservableProperty]
    private bool _isEditing;

    public ProfileViewModel(UserService userService)
    {
        _userService = userService;
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            Profile = await _userService.GetProfileAsync();
            if (Profile != null)
            {
                FirstName = Profile.FirstName;
                LastName = Profile.LastName;
                Phone = Profile.Phone ?? string.Empty;
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati profil.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ToggleEdit()
    {
        IsEditing = !IsEditing;
        SuccessMessage = string.Empty;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "Ime i prezime su obavezni.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            var request = new UpdateProfileRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone
            };
            Profile = await _userService.UpdateProfileAsync(request);
            SuccessMessage = "Profil je uspješno ažuriran.";
            IsEditing = false;
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće ažurirati profil.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
