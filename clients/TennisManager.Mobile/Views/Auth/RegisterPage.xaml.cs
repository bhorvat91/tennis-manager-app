using TennisManager.Mobile.ViewModels.Auth;

namespace TennisManager.Mobile.Views.Auth;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
