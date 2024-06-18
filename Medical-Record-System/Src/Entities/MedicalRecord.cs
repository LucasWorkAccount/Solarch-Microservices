namespace Medical_Record_System.Entities;

public class MedicalRecord
{
    public Guid Uuid { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Sex { get; set; }
    public string Bsn { get; set; }
    public string Record { get; set; }

    public MedicalRecord(Guid uuid, string name, int age, string sex, string bsn, string record)
    {
        Uuid = uuid;
        Name = name;
        Age = age;
        Sex = sex;
        Bsn = bsn;
        Record = record;
    }
}