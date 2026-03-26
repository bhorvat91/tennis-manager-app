using TennisManager.Mobile.ViewModels.Reservations;

namespace TennisManager.Mobile.Views.Reservations;

public partial class CourtListPage : ContentPage
{
    public CourtListPage(CourtListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
