namespace Appointment_Planner.Entities;

public partial class Appointment
{
    public Appointment(Guid patient, Guid doctor, DateTime datetime, string arrival, Guid referral)
    {
        Patient = patient;
        Doctor = doctor;
        Datetime = datetime;
        Arrival = arrival;
        Referral = referral;
    }
    
    public int Id { get; set; }

    public Guid Patient { get; set; }

    public Guid Doctor { get; set; }

    public DateTime Datetime { get; set; }

    public string Arrival { get; set; } = null!;

    public Guid Referral { get; set; }
}
