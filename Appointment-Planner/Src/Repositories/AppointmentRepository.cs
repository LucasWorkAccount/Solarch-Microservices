using Appointment_Planner.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appointment_Planner.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentPlannerDbContext _appointmentPlannerDbContext;

    public AppointmentRepository(AppointmentPlannerDbContext appointmentPlannerDbContext)
    {
        _appointmentPlannerDbContext = appointmentPlannerDbContext;
    }

    public async Task<Guid> CreateAppointment(Guid referral)
    {
        await _appointmentPlannerDbContext.Appointments.AddAsync(new Appointment(
            Guid.Empty,
            Guid.Empty,
            DateTime.MinValue,
            "NotYet",
            referral
        ));

        await _appointmentPlannerDbContext.SaveChangesAsync();
        return referral;
    }

    public async Task<Appointment> PlanAppointment(Appointment appointment)
    {
        var referredAppointment =
            await _appointmentPlannerDbContext.Appointments.FirstOrDefaultAsync(a =>
                a.Referral == appointment.Referral);

        if (referredAppointment == null)
        {
            throw new Exception("Appointment referral not found!");
        }

        referredAppointment.Patient = appointment.Patient;
        referredAppointment.Doctor = appointment.Doctor;
        referredAppointment.Datetime = appointment.Datetime;
        referredAppointment.Arrival = appointment.Arrival;
        await _appointmentPlannerDbContext.SaveChangesAsync();

        return referredAppointment;
    }

    public async Task<Appointment> RescheduleAppointment(Guid referral, DateTime newDateTime)
    {
        var newAppointment =
            await _appointmentPlannerDbContext.Appointments.FirstOrDefaultAsync(a => a.Referral == referral);

        if (newAppointment == null)
        {
            throw new Exception("Appointment not found!");
        }

        newAppointment.Datetime = newDateTime;
        await _appointmentPlannerDbContext.SaveChangesAsync();

        return newAppointment;
    }

    public async Task<List<Appointment>> GetAppointmentsForPatient(Guid patientUuid)
    {
        return await _appointmentPlannerDbContext.Appointments
            .Where(a => a.Patient == patientUuid)
            .OrderBy(a => a.Datetime).ToListAsync();
    }

    public async Task<Appointment> PlanFollowupAppointment(Guid referral, DateTime dateTime)
    {
        var previousAppointment =
            await _appointmentPlannerDbContext.Appointments.FirstOrDefaultAsync(a => a.Referral == referral);
        
        if (previousAppointment == null)
        {
            throw new Exception("Appointment not found!");
        }
        
        var followupAppointment = new Appointment(
            previousAppointment.Patient,
            previousAppointment.Doctor,
            dateTime,
            "NotYet",
            Guid.NewGuid()
        );
        
        await _appointmentPlannerDbContext.Appointments.AddAsync(followupAppointment);
        await _appointmentPlannerDbContext.SaveChangesAsync();
        
        return followupAppointment;
    }
    
    public async Task<Appointment> SetArrival(Guid referral, Arrival arrival)
    {
        var appointment = await _appointmentPlannerDbContext.Appointments.FirstOrDefaultAsync(a => a.Referral == referral);
        
        if (appointment == null)
        {
            throw new Exception("Appointment not found!");
        }
        
        appointment.Arrival = arrival.ToString();
        await _appointmentPlannerDbContext.SaveChangesAsync();
        
        return appointment;
    }
}