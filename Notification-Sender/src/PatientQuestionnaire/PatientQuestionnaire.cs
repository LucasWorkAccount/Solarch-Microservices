using System;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using PatientManagement;

class PatientQuestionnaire : IQuestionnaireSender
{
    private ConnectionFactory _factory;

    public PatientQuestionnaire()
    {
        _factory = new ConnectionFactory();
        _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
        _factory.ClientProvidedName = "PatientQuestionnaire Sender App";
    }
    
    public void Send(string queueName, Questionnaire questionnaire)
    {
        Thread.Sleep(30000);
        
        using IConnection connection = _factory.CreateConnection();
        using IModel channel = connection.CreateModel();

        string exchangeName = "PatientQuestionnaireExchange";
        string routingKey = "PatientQuestionnaire-route-key";
        
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        channel.QueueDeclare(queueName, false, false, false, null);
        channel.QueueBind(queueName, exchangeName, routingKey, null);
        
        var jason = JsonSerializer.Serialize(questionnaire);
        Console.WriteLine(jason);
        
        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(jason);
        
        channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

        channel.Close();
        connection.Close();
    }
}