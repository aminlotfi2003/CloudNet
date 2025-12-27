namespace CloudNet.Web.Services.ApiClients;

public sealed class ApiCookieHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiCookieHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            var cookieHeader = httpContext.Request.Headers.Cookie.ToString();
            if (!string.IsNullOrWhiteSpace(cookieHeader) && !request.Headers.Contains("Cookie"))
            {
                request.Headers.Add("Cookie", cookieHeader);
            }
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (httpContext is not null && response.Headers.TryGetValues("Set-Cookie", out var setCookies))
        {
            foreach (var setCookie in setCookies)
            {
                httpContext.Response.Headers.Append("Set-Cookie", setCookie);
            }
        }

        return response;
    }
}
