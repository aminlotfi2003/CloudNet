namespace CloudNet.Domain.Common;

public abstract class EntityBase<TId>
{
    public TId Id { get; protected set; } = default!;
}
