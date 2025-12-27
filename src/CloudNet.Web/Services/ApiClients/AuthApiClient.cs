using CloudNet.Web.Services.Models.AuthModels;

namespace CloudNet.Web.Services.ApiClients;

public sealed class AuthApiClient : ApiClientBase
{
    public AuthApiClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/register")
        {
            Content = JsonContent.Create(request)
        };
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<AuthResponse>(httpRequest, ct);
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/login")
        {
            Content = JsonContent.Create(request)
        };
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<AuthResponse>(httpRequest, ct);
    }

    public async Task<ApiResponse> LogoutAsync(string refreshToken, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/logout")
        {
            Content = JsonContent.Create(new LogoutRequest { RefreshToken = refreshToken })
        };
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync(httpRequest, ct);
    }
}
