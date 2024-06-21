using System.Text;
using System.Text.Json.Nodes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notification_Sender.RabbitMqReceivers;

public class GeneralPractitionerResultsReceiver
{

    public GeneralPractitionerResultsReceiver()
    {
    }

    public void Receiver()
    {

        try
        {

            Thread.Sleep(30000);
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
            factory.ClientProvidedName = "General practitioner results receiver App";

            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();



            string exchangeName = "General-practitioner-results-exchange";
            string routingKey = "General-practitioner-results-route-key";
            string queueName = "General-practitioner-results";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received Patient results: {message}");
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
