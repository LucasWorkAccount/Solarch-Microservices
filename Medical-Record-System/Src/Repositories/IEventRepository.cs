namespace Medical_Record_System.Repositories;

public interface IEventRepository
{
    public Task CreateEvent(Event @event);
    public Task<IEnumerable<Event>> GetEventsByUuid(Guid uuid);
}