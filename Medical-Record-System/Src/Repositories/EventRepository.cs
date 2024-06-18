using System.Text.Json.Nodes;
using Medical_Record_System.Events;
using Microsoft.EntityFrameworkCore;

namespace Medical_Record_System.Repositories;

public class EventRepository : IEventRepository
{
    private readonly MedicalRecordEventStoreContext _medicalRecordEventStoreContext;

    public EventRepository(MedicalRecordEventStoreContext medicalRecordEventStoreContext)
    {
        _medicalRecordEventStoreContext = medicalRecordEventStoreContext;
    }
    
    public async Task CreateEvent(Event @event)
    {
        await _medicalRecordEventStoreContext.Events.AddAsync(@event);
        await _medicalRecordEventStoreContext.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<Event>> GetEventsByUuid(Guid uuid)
    {
        var events = new List<Event>();
        foreach (var @event in await _medicalRecordEventStoreContext.Events.Where(@event => @event.Uuid == uuid).ToListAsync())
        {
            var json = JsonNode.Parse(@event.Body);
            switch (@event.Type)
            {
                case "MedicalRecordCreated":
                    events.Add(new MedicalRecordCreated(
                        @event.Uuid,
                        json!["name"]!.ToString(),
                        int.Parse(json!["age"]!.ToString()),
                        json!["sex"]!.ToString(),
                        json!["bsn"]!.ToString()
                    ));
                    break;
                case "MedicalRecordAppendage":
                    events.Add(new MedicalRecordAppendage(
                        @event.Uuid,
                        json!["entry"]!.ToString()
                    ));
                    break;
            }
        }

        return events;
    }
}