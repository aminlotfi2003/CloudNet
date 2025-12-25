namespace CloudNet.Api.Abstractions.Options;

public sealed class RateLimitPolicyOptions
{
    public int PermitLimit { get; init; } = 60;
    public int WindowSeconds { get; init; } = 60;
    public int QueueLimit { get; init; } = 0;
}
