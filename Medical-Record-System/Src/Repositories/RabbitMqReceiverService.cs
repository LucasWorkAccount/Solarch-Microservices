using System.Text;
using System.Text.Json.Nodes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Medical_Record_System.Repositories;

public class RabbitMqReceiverService: IRabbitMqReceiverService
{

    private IEventRepository _eventRepository;

    public RabbitMqReceiverService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public void Receiver()
    {

        try
        {

            Thread.Sleep(30000);
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
            factory.ClientProvidedName = "UserRegistration Sender App";

            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();



            string exchangeName = "User-registration-exchange";
            string routingKey = "User-registration-route-key";
            string queueName = "Medical-record-system-register";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);
                var json = JsonNode.Parse(message);
                var uuid = Guid.NewGuid();
                json!["uuid"] = uuid.ToString();

                var @event = new Event
                {
                    Uuid = uuid,
                    Body = json.ToJsonString(),
                    Type = "MedicalRecordCreated",
                    InsertedAt = DateTime.Now
                };
                _eventRepository.CreateEvent(@event);
                channel.BasicAck(args.DeliveryTag, false);
            };

            channel.BasicConsume(queueName, false, consumer);

            var waitHandle = new ManualResetEvent(false);
            waitHandle.WaitOne();
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
