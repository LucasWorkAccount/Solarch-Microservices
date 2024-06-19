namespace Medical_Record_System.Events;

public class MedicalRecordAppendage : Event
{
    public new Guid Uuid;
    public string Entry;

    public MedicalRecordAppendage(Guid uuid, string entry)
    {
        Uuid = uuid;
        Entry = entry;
    }
}