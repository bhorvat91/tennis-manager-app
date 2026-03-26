using TennisManager.Mobile.ViewModels.Matches;

namespace TennisManager.Mobile.Views.Matches;

public partial class MyMatchesPage : ContentPage
{
    private readonly MyMatchesViewModel _viewModel;

    public MyMatchesPage(MyMatchesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadMyMatchesCommand.Execute(null);
    }
}
