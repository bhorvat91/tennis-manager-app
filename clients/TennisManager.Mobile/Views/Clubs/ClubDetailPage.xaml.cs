using TennisManager.Mobile.ViewModels.Clubs;

namespace TennisManager.Mobile.Views.Clubs;

public partial class ClubDetailPage : ContentPage
{
    public ClubDetailPage(ClubDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
