using TennisManager.Mobile.ViewModels.Matches;

namespace TennisManager.Mobile.Views.Matches;

public partial class EnterResultPage : ContentPage
{
    public EnterResultPage(EnterResultViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
