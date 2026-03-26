using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Molimo unesite email i lozinku.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _authService.LoginAsync(new Models.LoginRequest
            {
                Email = Email,
                Password = Password
            });

            if (result != null)
                await Shell.Current.GoToAsync("//MyReservationsPage");
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Neispravni podaci za prijavu. Pokušajte ponovo.";
        }
        catch (Exception)
        {
            ErrorMessage = "Došlo je do pogreške. Pokušajte ponovo.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("//RegisterPage");
    }
}
