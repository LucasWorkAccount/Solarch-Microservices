using Appointment_Planner.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appointment_Planner.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentPlannerDbContext _dbContext;

    public AppointmentRepository(AppointmentPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Appointment>> GetAppointments(Guid? patientUuid)
    {
        if (patientUuid == null)
        {
            return await _dbContext.Appointments.ToListAsync();
        }

        return await _dbContext.Appointments
            .Where(a => a.Patient == patientUuid)
            .OrderBy(a => a.Datetime)
            .ToListAsync();
    }

    public async Task<Appointment?> GetAppointment(Guid referral)
    {
        return await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Referral == referral);
    }

    public async Task<Guid> CreateAppointment(Guid referral)
    {
        await _dbContext.Appointments.AddAsync(new Appointment(
            Guid.Empty,
            Guid.Empty,
            DateTime.MinValue,
            "NotYet",
            referral
        ));

        await _dbContext.SaveChangesAsync();
        return referral;
    }

    public async Task<Appointment?> EditAppointment(Appointment appointment)
    {
        var appointmentToEdit =
            await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Referral == appointment.Referral);

        if (appointmentToEdit == null)
        {
            return null;
        }

        appointmentToEdit.Patient = appointment.Patient;
        appointmentToEdit.Doctor = appointment.Doctor;
        appointmentToEdit.Datetime = appointment.Datetime;
        appointmentToEdit.Arrival = appointment.Arrival;

        await _dbContext.SaveChangesAsync();
        return appointmentToEdit;
    }

    public List<Appointment> GetAppointmentsNextDay(DateTime currentDay)
    {
        return _dbContext.Appointments.Where(a => a.Datetime.Day == currentDay.AddDays(1).Day)
            .ToList();
    }
}
