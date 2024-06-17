using RabbitMQ.Client;

namespace Patient_Management;

public class PatientQuestionnaireSender
{
    private ConnectionFactory _factory = new();

    public void QuestionnaireSender()
    {
        _factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        
    }
}