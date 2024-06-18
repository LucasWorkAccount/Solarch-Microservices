using System.Text.Json;

namespace Medical_Record_System;

public partial class MedicalRecord
{
    public int Id { get; set; }

    public Guid Uuid { get; set; }

    public string Name { get; set; } = null!;

    public int Age { get; set; }

    public string Sex { get; set; } = null!;

    public string Bsn { get; set; } = null!;

    public string Record { get; set; } = null!;
    
    public void Apply(Event @event)
    {
        var recordObject = JsonSerializer.Deserialize<RecordObject>(Record);
        if (recordObject?.entries == null)
        {
            throw new NullReferenceException("Medical record from DB could not be parsed.");
        }
        recordObject.entries.Add(@event.Body);
        Record = JsonSerializer.Serialize(recordObject);
    }

    private class RecordObject
    {
        public List<object>? entries { get; set; }
    }
}
