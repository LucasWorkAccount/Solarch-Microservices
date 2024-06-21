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

app.MapGet("/appointments/{patientUuid}",
        async (string patientUuid, IAppointmentRepository appointmentRepository) =>
        {
            return Results.Ok(await appointmentRepository.GetAppointmentsForPatient(new Guid(patientUuid)));
        })
    .WithName("GetAppointmentsByPatient")
    .WithOpenApi();

app.MapPost("/appointments",
        async (IAppointmentRepository appointmentRepository) =>
        {
            var referral = Guid.NewGuid();

            await appointmentRepository.CreateAppointment(referral);
            return Results.Ok("Appointment referral created successfully! Code: \"" + referral +
                              "\" can be used to plan an appointment.");
        })
    .WithName("CreateAppointmentReferral")
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

            await appointmentRepository.PlanAppointment(appointment);
            return Results.Ok("Appointment planned successfully!");
        })
    .WithName("PlanAppointmentByReferral")
    .WithOpenApi();


app.MapPost("/research-results", (ResearchResults researchResults, IGeneralPractitionerResultsSenderService sender) =>
        {
            try
            {
                sender.Send("General-practitioner-results", JsonSerializer.Serialize(researchResults));
                return Results.Ok("Sent email to general practitioner!");
            }
            catch
            {
                return Results.BadRequest();
            }
        })
    .WithName("SendResearchResults")
    .WithOpenApi();


app.MapPut("/appointments/{referral}",
        async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository) =>
        {
            using var reader = new StreamReader(request.Body);
            var json = JsonNode.Parse(await reader.ReadToEndAsync());
            
            await appointmentRepository.RescheduleAppointment(new Guid(referral), DateTime.Parse(json!["datetime"]!.ToString()));
            return Results.Ok("Appointment date and time moved successfully!");
        })
    .WithName("RescheduleAppointmentByReferral")
    .WithOpenApi();

app.Run();

