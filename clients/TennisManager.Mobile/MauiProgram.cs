using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TennisManager.Mobile.Services;
using TennisManager.Mobile.ViewModels.Auth;
using TennisManager.Mobile.ViewModels.Clubs;
using TennisManager.Mobile.ViewModels.Matches;
using TennisManager.Mobile.ViewModels.Notifications;
using TennisManager.Mobile.ViewModels.Profile;
using TennisManager.Mobile.ViewModels.Reservations;
using TennisManager.Mobile.Views.Auth;
using TennisManager.Mobile.Views.Clubs;
using TennisManager.Mobile.Views.Matches;
using TennisManager.Mobile.Views.Notifications;
using TennisManager.Mobile.Views.Profile;
using TennisManager.Mobile.Views.Reservations;

namespace TennisManager.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var apiBaseUrl = "https://localhost:5001/api/v1/";

        // Register HTTP client with JWT handler
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddHttpClient("TennisManagerApi", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddSingleton<ITokenService, TokenService>();

        // Register services
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ClubService>();
        builder.Services.AddSingleton<MemberService>();
        builder.Services.AddSingleton<CourtService>();
        builder.Services.AddSingleton<ReservationService>();
        builder.Services.AddSingleton<MatchService>();
        builder.Services.AddSingleton<LeagueService>();
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<UserService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();

        builder.Services.AddTransient<ClubListViewModel>();
        builder.Services.AddTransient<ClubDetailViewModel>();
        builder.Services.AddTransient<MyClubsViewModel>();

        builder.Services.AddTransient<CourtListViewModel>();
        builder.Services.AddTransient<CourtAvailabilityViewModel>();
        builder.Services.AddTransient<CreateReservationViewModel>();
        builder.Services.AddTransient<MyReservationsViewModel>();
        builder.Services.AddTransient<ReservationDetailViewModel>();

        builder.Services.AddTransient<MatchListViewModel>();
        builder.Services.AddTransient<MatchDetailViewModel>();
        builder.Services.AddTransient<CreateMatchViewModel>();
        builder.Services.AddTransient<EnterResultViewModel>();
        builder.Services.AddTransient<MyMatchesViewModel>();
        builder.Services.AddTransient<PlayerStatsViewModel>();

        builder.Services.AddTransient<NotificationListViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Register Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<ClubListPage>();
        builder.Services.AddTransient<ClubDetailPage>();
        builder.Services.AddTransient<MyClubsPage>();

        builder.Services.AddTransient<CourtListPage>();
        builder.Services.AddTransient<CourtAvailabilityPage>();
        builder.Services.AddTransient<CreateReservationPage>();
        builder.Services.AddTransient<MyReservationsPage>();
        builder.Services.AddTransient<ReservationDetailPage>();

        builder.Services.AddTransient<MatchListPage>();
        builder.Services.AddTransient<MatchDetailPage>();
        builder.Services.AddTransient<CreateMatchPage>();
        builder.Services.AddTransient<EnterResultPage>();
        builder.Services.AddTransient<MyMatchesPage>();
        builder.Services.AddTransient<PlayerStatsPage>();

        builder.Services.AddTransient<NotificationListPage>();
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
