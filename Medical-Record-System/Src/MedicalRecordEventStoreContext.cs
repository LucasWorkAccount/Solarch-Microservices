using Microsoft.EntityFrameworkCore;

namespace Medical_Record_System;

public partial class MedicalRecordEventStoreContext : DbContext
{
    private const string ConnectionString = "Host=medical-record-event-store;Port=5432;Database=medical-record-event-store;Username=postgres;Password=1234";

    public MedicalRecordEventStoreContext()
    {
    }

    public MedicalRecordEventStoreContext(DbContextOptions<MedicalRecordEventStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("events_pkey");

            entity.ToTable("events");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Body)
                .HasColumnType("jsonb")
                .HasColumnName("body");
            entity.Property(e => e.InsertedAt)
                .HasDefaultValueSql("statement_timestamp()")
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("inserted_at");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Uuid).HasColumnName("uuid");
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("medical_record_pk");

            entity.ToTable("medical_record");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Bsn)
                .HasMaxLength(9)
                .HasColumnName("bsn");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Record)
                .HasColumnType("jsonb")
                .HasColumnName("record");
            entity.Property(e => e.Sex)
                .HasMaxLength(1)
                .HasColumnName("sex");
            entity.Property(e => e.Uuid).HasColumnName("uuid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}