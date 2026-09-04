namespace Dynasty.Carrington.Core.Domain;

/// <summary>
///     Base class for entities identified by <typeparamref name="TId" />.
/// </summary>
public abstract class Entity<TId>
    where TId : notnull
{
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>
    ///     Parameterless constructor used by persistence frameworks when materialising the entity.
    /// </summary>
    protected Entity()
    {
        Id = default!;
    }

    public TId Id { get; protected set; }
}