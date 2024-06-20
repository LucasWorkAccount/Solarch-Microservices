using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Medical_Record_System.Repositories;

public class RabbitMqReceiverService: IRabbitMqReceiverService
{
    private ConnectionFactory _factory;

    public RabbitMqReceiverService()
    {
        _factory = new ConnectionFactory();
    }

    public void Receiver()
    {

        try
        {
            Thread.Sleep(30000);
            _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
            _factory.ClientProvidedName = "UserRegistration Sender App";

            using IConnection connection = _factory.CreateConnection();
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

                string message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);

                channel.BasicAck(args.DeliveryTag, false);
            };

            string consumerTag = channel.BasicConsume(queueName, false, consumer);

            // Console.ReadLine();
            channel.BasicCancel(consumerTag);

            channel.Close();
            connection.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
