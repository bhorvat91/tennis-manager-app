namespace TennisManager.Mobile.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public AuthHeaderHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _tokenService.RemoveTokenAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }

        return response;
    }
}
