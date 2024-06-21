using System;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

class PatientQuestionnaire
{
    private ConnectionFactory _factory;

    public PatientQuestionnaire()
    {
        _factory = new ConnectionFactory();
    }
    
    public void Sender()
    {
        Thread.Sleep(40000);
        _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
        _factory.ClientProvidedName = "PatientQuestionnaire Sender App";
        
        using IConnection connection = _factory.CreateConnection();
        using IModel channel = connection.CreateModel();

        string exchangeName = "PatientQuestionnaireExchange";
        string routingKey = "PatientQuestionnaire-route-key";
        string queueName = "PatientQuestionnaireQueue";
        
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        channel.QueueDeclare(queueName, false, false, false, null);
        channel.QueueBind(queueName, exchangeName, routingKey, null);

        var questionnaire = new
        {
            uuid = "48726af9-c2fc-46fa-80f6-df8a57491b02",
            questions = new string[]
            {
                "How much alcohol do you consume weekly?",
                "How much do you smoke? ",
                "How much do you exercise weekly?"
            },
            answers = new string[]
            {
                "1-2 glasses a week",
                "No",
                "3-4 times a week"
            }
        };
        var jason = JsonSerializer.Serialize(questionnaire);
        Console.WriteLine(jason);
        
        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(jason);
        
        channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

        channel.Close();
        connection.Close();
    }
}