using Appointment_Planner.Entities;

namespace Appointment_Planner.Repositories;

public interface IAppointmentRepository
{
    public Task<Appointment?> GetAppointment(Guid referral);
    public Task<List<Appointment>> GetAppointments(Guid? patientUuid);
    public Task<Guid> CreateAppointment(Guid referral);
    public Task<Appointment?> EditAppointment(Appointment appointment);
}
