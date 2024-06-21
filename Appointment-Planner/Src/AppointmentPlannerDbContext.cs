using Appointment_Planner.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appointment_Planner;

public partial class AppointmentPlannerDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public AppointmentPlannerDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AppointmentPlannerDbContext(DbContextOptions<AppointmentPlannerDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("appointment-planner-db-npgsql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Arrival)
                .HasMaxLength(100)
                .HasColumnName("arrival");
            entity.Property(e => e.Datetime).HasColumnName("datetime");
            entity.Property(e => e.Doctor).HasColumnName("doctor");
            entity.Property(e => e.Patient).HasColumnName("patient");
            entity.Property(e => e.Referral).HasColumnName("referral");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
