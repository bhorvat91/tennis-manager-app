using TennisManager.Mobile.ViewModels.Auth;

namespace TennisManager.Mobile.Views.Auth;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
