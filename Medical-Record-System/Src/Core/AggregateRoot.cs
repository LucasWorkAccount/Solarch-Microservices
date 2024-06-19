namespace Medical_Record_System.Core;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<Event> _events;
    private bool IsReplaying { get; set; } = false;
    public int Version { get; private set; }
    public int OriginalVersion { get; private set; }

    public AggregateRoot(TId id) : base(id)
    {
        OriginalVersion = 0;
        Version = 0;
        _events = new List<Event>();
    }
    
    public AggregateRoot(TId id, IEnumerable<Event> events) : this(id)
    {
        IsReplaying = true;
        foreach (Event e in events)
        {
            When(e);
            OriginalVersion++;
            Version++;
        }
        IsReplaying = false;
    }
    
    public IEnumerable<Event> GetEvents()
    {
        return _events;
    }
    
    protected void RaiseEvent(Event @event)
    {
        When(@event);

        _events.Add(@event);
        Version += 1;
    }
    
    public void ClearEvents()
    {
        _events.Clear();
    }
    
    protected abstract void When(dynamic @event);
}