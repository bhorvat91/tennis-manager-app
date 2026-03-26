using TennisManager.Mobile.ViewModels.Reservations;

namespace TennisManager.Mobile.Views.Reservations;

public partial class ReservationDetailPage : ContentPage
{
    public ReservationDetailPage(ReservationDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
