using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Medical_Record_System.Repositories;

public class RabbitMqReceiverService: IRabbitMqReceiverService
{
    private ConnectionFactory _factory;
    private EventRepository _eventRepository;

    private IModel _channel;
    private IConnection _connection;

    private string _routingKey = "User-registration-route-key";
    private string _exchangeName = "User-registration-exchange";

    public RabbitMqReceiverService()
    {
        _factory = new ConnectionFactory();
        _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
        _factory.ClientProvidedName = "UserRegistration Sender App";
    }

    public void Receive(string queueName)
    {

        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(queueName, false, false, false, null);
        _channel.QueueBind(queueName, _exchangeName, _routingKey, null);
        _channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (sender, args) =>
        {
            var body = args.Body.ToArray();

            string message = Encoding.UTF8.GetString(body);
            Event e = JsonSerializer.Deserialize<Event>(message)!;
            await _eventRepository.CreateEvent(e);

            _channel.BasicAck(args.DeliveryTag, false);
        };

        string consumerTag = _channel.BasicConsume(queueName, false, consumer);

        Console.ReadLine();
        _channel.BasicCancel(consumerTag);

    }
}
