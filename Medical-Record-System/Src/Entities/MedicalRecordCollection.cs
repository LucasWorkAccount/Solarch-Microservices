using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Medical_Record_System.Core;
using Medical_Record_System.Events;

namespace Medical_Record_System.Entities;

public class MedicalRecordCollection : AggregateRoot<Guid>
{
    public List<MedicalRecord> MedicalRecords = [];
    
    public MedicalRecordCollection(Guid uuid, IEnumerable<Event> events) : base(uuid, events) { }
    
    protected override void When(dynamic @event)
    {
        Handle(@event);
    }
    
    private void Handle(MedicalRecordCreated @event)
    {
        var medicalRecord = new MedicalRecord(@event.Uuid, @event.Name, @event.Age, @event.Sex, @event.Bsn, @event.Record);
        MedicalRecords.Add(medicalRecord);
    }
    
    private void Handle(MedicalRecordAppendage @event)
    {
        var medicalRecord = MedicalRecords.FirstOrDefault(medicalRecord => medicalRecord.Uuid == @event.Uuid)!;
        medicalRecord.Record.Add(@event.Entry);
    }
}
