namespace Medical_Record_System;

public partial class Event
{
    public int Id { get; set; }

    public Guid Uuid { get; set; }

    public string Type { get; set; } = null!;

    public string Body { get; set; } = null!;

    public DateTime InsertedAt { get; set; }
}
