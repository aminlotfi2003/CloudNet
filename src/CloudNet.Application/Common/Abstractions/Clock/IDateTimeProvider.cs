namespace CloudNet.Application.Common.Abstractions.Clock;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
