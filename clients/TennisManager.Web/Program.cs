using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TennisManager.Web;
using TennisManager.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001/api/v1";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl + "/") });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<ClubService>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<CourtService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddScoped<LeagueService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<UserService>();

await builder.Build().RunAsync();
