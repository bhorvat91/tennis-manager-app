using TennisManager.Mobile.ViewModels.Matches;

namespace TennisManager.Mobile.Views.Matches;

public partial class CreateMatchPage : ContentPage
{
    public CreateMatchPage(CreateMatchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
