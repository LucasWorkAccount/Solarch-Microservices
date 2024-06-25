﻿using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Medical_Record_System.RabbitMq;

public class RabbitMQTransferralPatientReceiver
{
    private ConnectionFactory _factory;

    public RabbitMQTransferralPatientReceiver()
    {
        _factory = new ConnectionFactory();
    }

    public void Receiver()
    {
        Thread.Sleep(30000);
        Console.WriteLine("In receiver");
        
        _factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
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
        consumer.Received += (sender, args) =>
        {
            var body = args.Body.ToArray();

            string message = Encoding.UTF8.GetString(body);

            channel.BasicAck(args.DeliveryTag, false);
        };

        string consumerTag = channel.BasicConsume(queueName, false, consumer);

        Console.ReadLine();
        channel.BasicCancel(consumerTag);

        channel.Close();
        connection.Close();
    }
}