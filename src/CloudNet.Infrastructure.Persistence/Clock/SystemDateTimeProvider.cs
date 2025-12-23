using CloudNet.Application.Common.Abstractions.Clock;

namespace CloudNet.Infrastructure.Persistence.Clock;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
