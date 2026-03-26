using TennisManager.Mobile.Services;
using TennisManager.Mobile.Views.Auth;
using TennisManager.Mobile.Views.Clubs;
using TennisManager.Mobile.Views.Matches;
using TennisManager.Mobile.Views.Notifications;
using TennisManager.Mobile.Views.Reservations;

namespace TennisManager.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for detail/navigation pages
        Routing.RegisterRoute(nameof(ClubListPage), typeof(ClubListPage));
        Routing.RegisterRoute(nameof(ClubDetailPage), typeof(ClubDetailPage));
        Routing.RegisterRoute(nameof(CourtListPage), typeof(CourtListPage));
        Routing.RegisterRoute(nameof(CourtAvailabilityPage), typeof(CourtAvailabilityPage));
        Routing.RegisterRoute(nameof(CreateReservationPage), typeof(CreateReservationPage));
        Routing.RegisterRoute(nameof(ReservationDetailPage), typeof(ReservationDetailPage));
        Routing.RegisterRoute(nameof(MatchListPage), typeof(MatchListPage));
        Routing.RegisterRoute(nameof(MatchDetailPage), typeof(MatchDetailPage));
        Routing.RegisterRoute(nameof(CreateMatchPage), typeof(CreateMatchPage));
        Routing.RegisterRoute(nameof(EnterResultPage), typeof(EnterResultPage));
        Routing.RegisterRoute(nameof(PlayerStatsPage), typeof(PlayerStatsPage));
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var tokenService = Handler?.MauiContext?.Services.GetService<ITokenService>();
        if (tokenService != null)
            await tokenService.RemoveTokenAsync();

        await Shell.Current.GoToAsync("//LoginPage");
    }
}
