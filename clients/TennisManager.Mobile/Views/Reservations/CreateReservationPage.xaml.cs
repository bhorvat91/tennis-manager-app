using TennisManager.Mobile.ViewModels.Reservations;

namespace TennisManager.Mobile.Views.Reservations;

public partial class CreateReservationPage : ContentPage
{
    public CreateReservationPage(CreateReservationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
