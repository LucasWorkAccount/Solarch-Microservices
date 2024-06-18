using Medical_Record_System;
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
builder.Services.AddScoped<IEventHandler, StateEventHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/medical-records", async (MedicalRecordEventStoreContext dbContext) =>
    {
        var medicalRecords = await dbContext.MedicalRecords.ToListAsync();
        return Results.Ok(medicalRecords);
    })
    .WithName("GetMedicalRecords")
    .WithOpenApi();

app.MapGet("/medical-records/{uuid}", async (string uuid, MedicalRecordEventStoreContext dbContext) =>
    {
        Guid guid;
        try {
            guid = new Guid(uuid);
        }
        catch (Exception)
        {
            throw new ArgumentException("Given uuid is not valid!");
        }
        var medicalRecord = await dbContext.MedicalRecords.FirstOrDefaultAsync(e => e.Uuid == guid);
        return Results.Ok(medicalRecord);
    })
    .WithName("GetMedicalRecordByUuid")
    .WithOpenApi();

app.MapPost("medical-records/{uuid}", async (string uuid, HttpRequest request, MedicalRecordEventStoreContext dbContext, IEventHandler statesEventHandler) =>
    {
        using var reader = new StreamReader(request.Body);
        
        var @event = new Event
        {
            Uuid = new Guid(uuid),
            Body = await reader.ReadToEndAsync(),
            Type = "MedicalRecordAppendage",
            InsertedAt = DateTime.Now
        };
        await dbContext.Events.AddAsync(@event);
        await dbContext.SaveChangesAsync();
        
        await statesEventHandler.Apply(@event);
        return Results.Ok("Appended medical record successfully!");
    })
    .WithName("AppendMedicalRecord")
    .WithOpenApi();

app.Run();