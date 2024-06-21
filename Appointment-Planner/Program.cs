using System.Text.Json;
using System.Text.Json.Nodes;
using Appointment_Planner;
using Appointment_Planner.Entities;
using Appointment_Planner.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var objBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
IConfiguration configManager = objBuilder.Build();
var connection = configManager.GetConnectionString("appointment-planner-db");

builder.Services.AddDbContext<AppointmentPlannerDbContext>(options => options.UseNpgsql(connection));
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

builder.Services.AddSingleton<IGeneralPractitionerResultsSenderService, GeneralPractitionerResultsSenderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/appointments",
        async (string? patientUuid, IAppointmentRepository appointmentRepository) =>
        {
            var appointments =
                await appointmentRepository.GetAppointments(patientUuid == null ? null : new Guid(patientUuid));

            return Results.Json(new
            {
                status = 200,
                message = "Appointments retrieved successfully!",
                appointments
            });
        })
    .WithName("GetAppointments")
    .WithOpenApi();

app.MapPost("/appointments",
        async (IAppointmentRepository appointmentRepository) =>
        {
            var referral = Guid.NewGuid();

            await appointmentRepository.CreateAppointment(referral);
            return Results.Json(new
            {
                status = 200,
                message = "Referral created successfully!",
                referral
            });
        })
    .WithName("CreateAppointment")
    .WithOpenApi();


app.MapPost("/appointments/{referral}",
        async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository) =>
        {
            using var reader = new StreamReader(request.Body);
            var json = JsonNode.Parse(await reader.ReadToEndAsync());

            var appointment = new Appointment(
                new Guid(json!["patient"]!.ToString()),
                new Guid(json["doctor"]!.ToString()),
                DateTime.Parse(json!["datetime"]!.ToString()),
                json["arrival"]!.ToString(),
                new Guid(referral)
            );

            await appointmentRepository.EditAppointment(appointment);
            return Results.Json(new
            {
                status = 200,
                message = "Appointment planned successfully!",
                appointment
            });
        })
    .WithName("PlanAppointmentByReferral")
    .WithOpenApi();


app.MapPost("/appointments/followup/{referral}",
        async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository) =>
        {
            using var reader = new StreamReader(request.Body);
            var json = JsonNode.Parse(await reader.ReadToEndAsync());
            var appointment = await appointmentRepository.GetAppointment(new Guid(referral));

            if (appointment == null)
            {
                return Results.Json(new
                {
                    status = 404,
                    message = "Could not find an appointment with the given referral!",
                    referral
                });
            }

            var followupAppointment = new Appointment(
                appointment.Patient,
                appointment.Doctor,
                DateTime.Parse(json!["datetime"]!.ToString()),
                "NotYet",
                Guid.NewGuid()
            );

            await appointmentRepository.CreateAppointment(followupAppointment.Referral);
            await appointmentRepository.EditAppointment(followupAppointment);

            return Results.Json(new
            {
                status = 200,
                message = "Follow-up appointment planned successfully!",
                followupAppointment
            });
        })
    .WithName("PlanFollowupAppointmentByReferral")
    .WithOpenApi();


app.MapPut("/appointments/reschedule/{referral}",
        async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository) =>
        {
            using var reader = new StreamReader(request.Body);
            var json = JsonNode.Parse(await reader.ReadToEndAsync());
            var appointmentToEdit = await appointmentRepository.GetAppointment(new Guid(referral));

            if (appointmentToEdit == null)
            {
                return Results.Json(new
                {
                    status = 404,
                    message = "Could not find an appointment with the given referral!",
                    referral
                });
            }

            appointmentToEdit.Datetime = DateTime.Parse(json!["datetime"]!.ToString());
            await appointmentRepository.EditAppointment(appointmentToEdit);

            return Results.Json(new
            {
                status = 200,
                message = "Appointment rescheduled successfully!",
                appointmentToEdit
            });
        })
    .WithName("RescheduleAppointmentByReferral")
    .WithOpenApi();


app.MapPut("/appointments/arrival/{referral}",
        async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository) =>
        {
            using var reader = new StreamReader(request.Body);
            var json = JsonNode.Parse(await reader.ReadToEndAsync());
            var appointmentToEdit = await appointmentRepository.GetAppointment(new Guid(referral));
            
            if (appointmentToEdit == null)
            {
                return Results.Json(new
                {
                    status = 404,
                    message = "Could not find an appointment with the given referral!",
                    referral
                });
            }
            
            if (!Enum.TryParse(json!["arrival"]!.ToString(), out Arrival arrival))
            {
                return Results.Json(new
                {
                    status = 400,
                    message = "Invalid arrival status! Expected one of the following:",
                    validArrivalStatuses = Enum.GetNames(typeof(Arrival))
                });
            }
            
            appointmentToEdit.Arrival = arrival.ToString();
            await appointmentRepository.EditAppointment(appointmentToEdit);

            return Results.Json(new
            {
                status = 200,
                message = "Appointment arrival status updated successfully!",
                appointmentToEdit
            });
        })
    .WithName("SetAppointmentArrivalByReferral")
    .WithOpenApi();


app.MapPost("/research-results", (ResearchResults researchResults, IGeneralPractitionerResultsSenderService sender) =>
    {
        try
        {
            sender.Send("General-practitioner-results", JsonSerializer.Serialize(researchResults));
            return Results.Json(new
            {
                status = 200,
                message = "Email sent to general practitioner successfully!"
            });
        }
        catch
        {
            return Results.Json(new
            {
                status = 400,
                message = "Failed to send email to general practitioner!"
            });
        }
    })
    .WithName("SendResearchResults")
    .WithOpenApi();


app.Run();
