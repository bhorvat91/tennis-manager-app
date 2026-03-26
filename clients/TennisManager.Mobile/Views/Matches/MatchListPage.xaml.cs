using TennisManager.Mobile.ViewModels.Matches;

namespace TennisManager.Mobile.Views.Matches;

public partial class MatchListPage : ContentPage
{
    private readonly MatchListViewModel _viewModel;

    public MatchListPage(MatchListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadMatchesCommand.Execute(null);
    }
}
