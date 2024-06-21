using Appointment_Planner.Entities;

namespace Appointment_Planner.Repositories;

public interface IAppointmentRepository
{
    public Task<Guid> CreateAppointment(Guid referral);
    public Task<Appointment> PlanAppointment(Appointment appointment);
}
