using TennisManager.Mobile.ViewModels.Reservations;

namespace TennisManager.Mobile.Views.Reservations;

public partial class CourtAvailabilityPage : ContentPage
{
    private readonly CourtAvailabilityViewModel _viewModel;

    public CourtAvailabilityPage(CourtAvailabilityViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadSlotsCommand.Execute(null);
    }
}
