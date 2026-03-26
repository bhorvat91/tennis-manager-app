using TennisManager.Mobile.ViewModels.Matches;

namespace TennisManager.Mobile.Views.Matches;

public partial class PlayerStatsPage : ContentPage
{
    private readonly PlayerStatsViewModel _viewModel;

    public PlayerStatsPage(PlayerStatsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadStatsCommand.Execute(null);
    }
}
