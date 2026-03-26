using TennisManager.Mobile.ViewModels.Reservations;

namespace TennisManager.Mobile.Views.Reservations;

public partial class MyReservationsPage : ContentPage
{
    private readonly MyReservationsViewModel _viewModel;

    public MyReservationsPage(MyReservationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadReservationsCommand.Execute(null);
    }
}
