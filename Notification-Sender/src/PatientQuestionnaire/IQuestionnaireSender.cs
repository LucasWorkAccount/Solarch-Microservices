namespace PatientManagement;

public interface IQuestionnaireSender
{
    public void Send(string queueName, Questionnaire questionnaire);
}