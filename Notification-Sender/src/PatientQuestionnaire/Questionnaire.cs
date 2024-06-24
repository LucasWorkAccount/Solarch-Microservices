namespace PatientManagement;

public class Questionnaire
{
    public Guid uuid { get; set; }
    public string[] questions { get; set; }
    public string[] answers { get; set; }

    public Questionnaire()
    {
        
    }
    public Questionnaire(string[] questions, string[] answers)
    {
        this.questions = questions;
        this.answers = answers;
    }
    
    public Questionnaire(Guid uuid, string[] questions, string[] answers)
    {
        this.uuid = uuid;
        this.questions = questions;
        this.answers = answers;
    }
}