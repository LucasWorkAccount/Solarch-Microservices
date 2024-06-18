using RabbitMQ.Client;

namespace Medical_Record_System.RabbitMq;

public class PatientQuestionnaireReceiver
{
    private ConnectionFactory _factory;

    public PatientQuestionnaireReceiver()
    {
        _factory = new ConnectionFactory();
    }

    public void Receiver()
    {
        _factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        _factory.ClientProvidedName = "PatientQuestionnaire Sender App";
        
        using IConnection connection = _factory.CreateConnection();
        using IModel channel = connection.CreateModel();
    }
}