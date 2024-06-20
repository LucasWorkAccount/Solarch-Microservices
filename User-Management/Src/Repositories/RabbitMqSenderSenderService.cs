using System.Text;
using RabbitMQ.Client;
using User_Management.Model;

namespace User_Management.Repositories;

public class RabbitMqSenderSenderService: IRabbitMqSenderService
{
    private ConnectionFactory _factory;
    private IModel _channel;
    private IConnection _connection;
    private string _routingKey = "User-registration-route-key";
    private string _exchangeName = "User-registration-exchange";

    public RabbitMqSenderSenderService()
    {
        _factory = new ConnectionFactory();
        _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
        _factory.ClientProvidedName = "UserRegistration Sender App";
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
