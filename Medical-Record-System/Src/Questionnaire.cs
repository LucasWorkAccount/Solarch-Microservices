namespace Medical_Record_System;
public partial class Questionnaire
{
    public int Id { get; set; }

    public Guid Uuid { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public DateTime InsertedAt { get; set; }
}

public class QuestionnaireData
{
    public Guid Uuid { get; set; }
    public List<string> Questions { get; set; }
    public List<string> Answers { get; set; }
}