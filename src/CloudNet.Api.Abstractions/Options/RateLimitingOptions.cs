namespace CloudNet.Api.Abstractions.Options;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public RateLimitPolicyOptions PerUser { get; init; } = new()
    {
        PermitLimit = 60,
        WindowSeconds = 60,
        QueueLimit = 0
    };

    public RateLimitPolicyOptions AuthSensitive { get; init; } = new()
    {
        PermitLimit = 5,
        WindowSeconds = 60,
        QueueLimit = 0
    };

    public RateLimitPolicyOptions InvalidUser { get; init; } = new()
    {
        PermitLimit = 5,
        WindowSeconds = 60,
        QueueLimit = 0
    };

    public int DefaultRetryAfterSeconds { get; init; } = 10;
}
