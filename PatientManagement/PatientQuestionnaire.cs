using System;
using RabbitMQ.Client;
using System.Text;

class PatientQuestionnaire
{
    private ConnectionFactory _factory;

    public PatientQuestionnaire()
    {
        _factory = new ConnectionFactory();
    }
    
    public void Sender()
    {
        _factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        _factory.ClientProvidedName = "PatientQuestionnaire Sender App";
        
        using IConnection connection = _factory.CreateConnection();
        using IModel channel = connection.CreateModel();

        string exchangeName = "PatientQuestionnaireExchange";
        string routingKey = "PatientQuestionnaire-route-key";
        string queueName = "PatientQuestionnaireQueue";
        
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        channel.QueueDeclare(queueName, false, false, false, null);
        channel.QueueBind(queueName, exchangeName, routingKey, null);

        byte[] messageBodyBytes = Encoding.UTF8.GetBytes("test string");
        channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
        
        channel.Close();
        connection.Close();
    }
}