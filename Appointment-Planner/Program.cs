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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/appointment",
        async (IAppointmentRepository appointmentRepository) =>
        {
            var referral = Guid.NewGuid();
            
            await appointmentRepository.CreateAppointment(referral);
            return Results.Ok("Appointment referral created successfully! Code: \"" + referral + "\" can be used to plan an appointment.");
        })
    .WithName("CreateAppointmentReferral")
    .WithOpenApi();


app.MapPut("/appointment/{referral}",
        async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository) =>
        {
            using var reader = new StreamReader(request.Body);
            var json = JsonNode.Parse(await reader.ReadToEndAsync());
            DateTime utcDateTime = DateTime.SpecifyKind(DateTime.Parse(json!["datetime"]!.ToString()), DateTimeKind.Utc);

            var appointment = new Appointment(
                new Guid(json!["patient"]!.ToString()),
                new Guid(json["doctor"]!.ToString()),
                utcDateTime,
                json["arrival"]!.ToString(),
                new Guid(referral)
            );

            await appointmentRepository.PlanAppointment(appointment);
            return Results.Ok("Appointment planned successfully!");
        })
    .WithName("PlanAppointment")
    .WithOpenApi();

app.Run();