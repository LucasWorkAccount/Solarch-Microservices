using System.Text;
using RabbitMQ.Client;

namespace Appointment_Planner.Repositories;
public class GeneralPractitionerResultsSenderService: IGeneralPractitionerResultsSenderService
{
    private ConnectionFactory _factory;
    private IModel _channel;
    private IConnection _connection;
    private string _routingKey = "General-practitioner-results-route-key";
    private string _exchangeName = "General-practitioner-results-exchange";

    public GeneralPractitionerResultsSenderService()
    {
        _factory = new ConnectionFactory();
        _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
        _factory.ClientProvidedName = "General practitioner results receiver App";
    }

    public void Send(string queueName, string message)
    {
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(queueName, false, false, false, null);
        _channel.QueueBind(queueName,_exchangeName,_routingKey,null);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(_exchangeName, _routingKey, null, messageBytes);


        _channel.Close();
        _connection.Close();
    }
}
