namespace User_Management.Model;

public interface IRabbitMqSenderService
{
    public void Send(string queueName, string message);
}
