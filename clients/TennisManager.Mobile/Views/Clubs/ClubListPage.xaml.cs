using TennisManager.Mobile.ViewModels.Clubs;

namespace TennisManager.Mobile.Views.Clubs;

public partial class ClubListPage : ContentPage
{
    private readonly ClubListViewModel _viewModel;

    public ClubListPage(ClubListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadClubsCommand.Execute(null);
    }
}
