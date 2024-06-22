using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Appointment_Planner.Repositories;

public class AppointmentReminderSender : IAppointmentReminderSender
{
    private IAppointmentRepository _appointmentRepository;

    private ConnectionFactory _factory;
    private IModel _channel;
    private IConnection _connection;
    private string _routingKey = "Appointment-reminder-route-key";
    private string _exchangeName = "Appointment-reminder-exchange";

    public AppointmentReminderSender(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
        _factory = new ConnectionFactory();
        _factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
        _factory.ClientProvidedName = "Appointment reminder sender App";
    }

    public void Send(string queueName)
    {
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(queueName, false, false, false, null);
        _channel.QueueBind(queueName, _exchangeName, _routingKey, null);

        foreach (var appointment in RetrieveAndConvertAppointments())
        {
            _channel.BasicPublish(_exchangeName, _routingKey, null, appointment);
        }

        _channel.Close();
        _connection.Close();
    }


    private List<byte[]> RetrieveAndConvertAppointments()
    {
        Console.WriteLine(DateTime.Now);
        var appointments = _appointmentRepository.GetAppointmentsNextDay(DateTime.Now);
        return appointments.Select(appointment => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(appointment)))
            .ToList();
    }
}
