using System.Text;
using RabbitMQ.Client;

namespace Appointment_Planner.Repositories;
public class RabbitMqSenderService: IRabbitMqSenderService
{
    private ConnectionFactory _factory;
    private IModel _channel;
    private IConnection _connection;

    public RabbitMqSenderService()
    {
        _factory = new ConnectionFactory();
        _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
    }

    public void Send(string queueName, string message, string routingKey, string exchangeName, string clientProvidedName)
    {
        _factory.ClientProvidedName = clientProvidedName;
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(queueName, false, false, false, null);
        _channel.QueueBind(queueName,exchangeName,routingKey,null);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchangeName, routingKey, null, messageBytes);


        _channel.Close();
        _connection.Close();
    }
}
