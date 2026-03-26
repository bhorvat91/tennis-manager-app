using TennisManager.Mobile.ViewModels.Clubs;

namespace TennisManager.Mobile.Views.Clubs;

public partial class MyClubsPage : ContentPage
{
    private readonly MyClubsViewModel _viewModel;

    public MyClubsPage(MyClubsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadMyClubsCommand.Execute(null);
    }
}
