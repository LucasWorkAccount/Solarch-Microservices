namespace Medical_Record_System.Repositories;

public interface IRabbitMqReceiverService
{

    public void Receive(string queueName);
}
