namespace Appointment_Planner.Repositories;

public interface IAppointmentReminderSender
{
    public void Send(string queueName);
}
