namespace Appointment_Planner.Repositories;

public interface IRabbitMqSenderService
{

    public void Send(string queueName, string message, string routingKey, string exchangeName, string clientProvidedName);
}
