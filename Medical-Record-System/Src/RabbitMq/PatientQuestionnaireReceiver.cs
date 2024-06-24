using System.Text;
using System.Text.Json;
using Medical_Record_System.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Medical_Record_System.RabbitMq;

public class PatientQuestionnaireReceiver
{
    private ConnectionFactory _factory;
    private IEventRepository _repository;
    
    private IModel _channel;
    private IConnection _connection;

    public PatientQuestionnaireReceiver(IEventRepository eventRepository)
    {
        _factory = new ConnectionFactory();
        _repository = eventRepository;
    }

    public void Receiver()
    {
        Thread.Sleep(30000);
        _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
        _factory.ClientProvidedName = "PatientQuestionnaire Receiver App";

        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        string exchangeName = "PatientQuestionnaireExchange";
        string routingKey = "PatientQuestionnaire-route-key";
        string queueName = "PatientQuestionnaireQueue";
        
        _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(queueName, false, false, false, null);
        _channel.QueueBind(queueName, exchangeName, routingKey, null);
        _channel.BasicQos(0, 1, false);
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (sender, args) =>
        {
            var body = args.Body.ToArray();

            string message = Encoding.UTF8.GetString(body);
            QuestionnaireData data = JsonSerializer.Deserialize<QuestionnaireData>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            List<Questionnaire> questionnaires = TransformQuestionnaires(data);

            foreach (Questionnaire questionnaire in questionnaires)
            {
                var @event = new Event
                {
                    Uuid = questionnaire.Uuid,
                    Body = $"{{\"entry\" : \"question: {questionnaire.Question}, answers: {questionnaire.Answer}\"}}",
                    Type = "MedicalRecordAppendage",
                    InsertedAt = DateTime.Now
                };
                Console.WriteLine(@event.Uuid + " " + @event.Body + " " + @event.Type + " " + @event.InsertedAt);
                await _repository.CreateEvent(@event);
            }
            
            _channel.BasicAck(args.DeliveryTag, false);
        };

        string consumerTag = _channel.BasicConsume(queueName, false, consumer);

        var waitHandle = new ManualResetEvent(false);
        waitHandle.WaitOne();
        
        _channel.BasicCancel(consumerTag);
        _channel.Close();
        _connection.Close();
    }

    private List<Questionnaire> TransformQuestionnaires(QuestionnaireData data)
    {
        List<Questionnaire> questionnaires = new List<Questionnaire>();

        if (data.Questions.Count == data.Answers.Count)
        {
            for (int i = 0; i < data.Questions.Count; i++)
            {
                questionnaires.Add(new Questionnaire
                {
                    Uuid = data.Uuid,
                    Question = data.Questions[i],
                    Answer = data.Answers[i],
                    InsertedAt = DateTime.Now
                });
            }
        }

        return questionnaires;
    }    
}