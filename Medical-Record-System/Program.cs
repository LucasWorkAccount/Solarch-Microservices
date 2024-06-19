using System.Text.Json;
using System.Text.Json.Nodes;
using Medical_Record_System;
using Medical_Record_System.Entities;
using Medical_Record_System.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var objBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
IConfiguration configManager = objBuilder.Build();
var connection = configManager.GetConnectionString("medical-record-event-store");

builder.Services.AddDbContext<MedicalRecordEventStoreContext>(options => options.UseNpgsql(connection));
builder.Services.AddScoped<IEventRepository, EventRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/medical-records/{uuid}", async (string uuid, IEventRepository eventRepository) =>
    {
        Guid guid;
        try {
            guid = new Guid(uuid);
        }
        catch (Exception)
        {
            throw new ArgumentException("Given uuid is not valid!");
        }

        var medicalRecordEvents = await eventRepository.GetEventsByUuid(guid);
        var medicalRecord = new MedicalRecordCollection(guid, medicalRecordEvents).MedicalRecords[0];
        return Results.Ok(medicalRecord);
    })
    .WithName("GetMedicalRecordByUuid")
    .WithOpenApi();

app.MapPost("medical-records", async (HttpRequest request, IEventRepository eventRepository) =>
    {
        using var reader = new StreamReader(request.Body);
        var json = JsonNode.Parse(await reader.ReadToEndAsync());
        var uuid = Guid.NewGuid();
        json!["uuid"] = uuid.ToString();
        
        var @event = new Event
        {
            Uuid = uuid,
            Body = json.ToJsonString(),
            Type = "MedicalRecordCreated",
            InsertedAt = DateTime.Now
        };

        await eventRepository.CreateEvent(@event);
        
        return Results.Ok("Medical record successfully initialised!");
    })
    .WithName("CreateMedicalRecord")
    .WithOpenApi();

app.MapPut("medical-records/{uuid}", async (string uuid, HttpRequest request, IEventRepository eventRepository) =>
    {
        using var reader = new StreamReader(request.Body);
        var json = JsonNode.Parse(await reader.ReadToEndAsync());
        
        var @event = new Event
        {
            Uuid = new Guid(uuid),
            Body = json!.ToJsonString(),
            Type = "MedicalRecordAppendage",
            InsertedAt = DateTime.Now
        };
        
        await eventRepository.CreateEvent(@event);
        
        return Results.Ok("Medical record successfully appended!");
    })
    .WithName("AppendMedicalRecord")
    .WithOpenApi();

app.Run();