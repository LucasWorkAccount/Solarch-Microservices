namespace Appointment_Planner.Repositories;

public interface IGeneralPractitionerResultsSenderService
{

    public void Send(string queueName, string message);
}
