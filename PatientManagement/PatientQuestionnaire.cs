using RabbitMQ.Client;

namespace PatientManagement;

public class PatientQuestionnaire
{
    private ConnectionFactory _connectionFactory;

    PatientQuestionnaire()
    {
        _connectionFactory = new ConnectionFactory();
    }

    public void Sender()
    {
        _connectionFactory.Uri = new Uri("amqp://guest:guest@localhost:4420");
        _connectionFactory.ClientProvidedName = "Patient Questionnaire Sender App";

        using var connection = _connectionFactory.CreateConnectionAsync();
        //using var channel = connection.CreationOptions();
    }
}