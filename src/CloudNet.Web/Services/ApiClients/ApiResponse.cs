using System.Net;

namespace CloudNet.Web.Services.ApiClients;

public sealed class ApiResponse<T>
{
    public ApiResponse(T? result, HttpStatusCode statusCode)
    {
        Result = result;
        StatusCode = statusCode;
    }

    public ApiResponse(ApiProblemDetails problem, HttpStatusCode statusCode)
    {
        Problem = problem;
        StatusCode = statusCode;
    }

    public T? Result { get; }
    public ApiProblemDetails? Problem { get; }
    public HttpStatusCode StatusCode { get; }
    public bool IsSuccess => Problem is null && StatusCode is >= HttpStatusCode.OK and < HttpStatusCode.MultipleChoices;
}

public sealed class ApiResponse
{
    public ApiResponse(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public ApiResponse(ApiProblemDetails problem, HttpStatusCode statusCode)
    {
        Problem = problem;
        StatusCode = statusCode;
    }

    public ApiProblemDetails? Problem { get; }
    public HttpStatusCode StatusCode { get; }
    public bool IsSuccess => Problem is null && StatusCode is >= HttpStatusCode.OK and < HttpStatusCode.MultipleChoices;
}
