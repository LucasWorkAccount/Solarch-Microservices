using System.Text;
using System.Text.Json;
using Patient_Transferral_System.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using User_Management.Model;

namespace Medical_Record_System.RabbitMq;

public class RabbitMQTransferralPatientReceiver
{
    private ConnectionFactory _factory;
    private IUserRepository _userRepository;
    private IRabbitMqSenderService _rabbitMqService;

    public RabbitMQTransferralPatientReceiver(IUserRepository userRepository, IRabbitMqSenderService rabbitMqService)
    {
        _userRepository = userRepository;
        _rabbitMqService = rabbitMqService;
        _factory = new ConnectionFactory();
    }

    public async void Receiver()
    {
            Thread.Sleep(30000);

            _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
            _factory.ClientProvidedName = "Patient-Transferral-System";

            using IConnection connection = _factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            Console.WriteLine("Made connection");

            string exchangeName = "Patient-Transferral-exchange";
            string routingKey = "Patient-Transferral-route-key";
            string queueName = "Patient-Transferral-System";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, args) =>
            {
                var body = args.Body.ToArray();

                string message = Encoding.UTF8.GetString(body);

                TransferralPatient transferralPatient = JsonSerializer.Deserialize<TransferralPatient>(message);
                await addTransferralPatient(transferralPatient.FirstName, transferralPatient.LastName);
                
                channel.BasicAck(args.DeliveryTag, false);
            };

            string consumerTag = channel.BasicConsume(queueName, false, consumer);

            var waitHandle = new ManualResetEvent(false);
            waitHandle.WaitOne();

            channel.BasicCancel(consumerTag);
    }
    
    public async Task addTransferralPatient(string firstname, string lastname)
    {
        User newUser = new User();
        newUser.Uuid = Guid.NewGuid();
        newUser.Email = firstname + lastname + "@amphia.com";
        newUser.Password = BCrypt.Net.BCrypt.HashPassword("guest");
        newUser.Role = "Patient";
        if (await _userRepository.UserExists(newUser.Email))
        {
            Console.WriteLine("Email already exists in database");
        }
        else
        {
            var MRSUser = new
            {
                uuid = newUser.Uuid,
                name = firstname + " " + lastname,
                sex = "X",
                age = -1,
                bsn = "N/A"
            };
        
            await _userRepository.AddUser(newUser);
            _rabbitMqService.Send("Medical-record-system-register", JsonSerializer.Serialize(MRSUser));
        }
    }
}