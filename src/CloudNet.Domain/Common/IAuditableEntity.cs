namespace CloudNet.Domain.Common;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset? ModifiedAt { get; set; }
}
