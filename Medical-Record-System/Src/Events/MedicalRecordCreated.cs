namespace Medical_Record_System.Events;

public class MedicalRecordCreated : Event
{
    public new Guid Uuid;
    public string Name;
    public int Age;
    public string Sex;
    public string Bsn;
    public string Record;

    public MedicalRecordCreated(Guid uuid, string name, int age, string sex, string bsn)
    {
        Uuid = uuid;
        Name = name;
        Age = age;
        Sex = sex;
        Bsn = bsn;
        Record = "{\"entries\":[]}";
    }
}