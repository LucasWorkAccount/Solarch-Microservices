namespace Medical_Record_System.Core;

public class Entity<TId>
{
    public TId Id { get; private set; }

    public Entity(TId id)
    {
        Id = id;
    }
}