using System.Text;
using System.Text.Json.Nodes;
using Medical_Record_System;
using Medical_Record_System.RabbitMq;
using Medical_Record_System.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var objBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
IConfiguration configManager = objBuilder.Build();
var connection = configManager.GetConnectionString("medical-record-event-store");

builder.Services.AddDbContext<MedicalRecordEventStoreContext>(options => options.UseNpgsql(connection));
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

builder.Services.AddSingleton<PatientQuestionnaireReceiver>();
builder.Services.AddSingleton<IRabbitMqReceiverService, RabbitMqReceiverService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/medical-records/{uuid}",[Authorize(Roles = "Doctor")] async (string uuid, IMedicalRecordRepository medicalRecordRepository) =>
    {
        return Results.Ok(await medicalRecordRepository.GetMedicalRecordByUuid(new Guid(uuid)));
    })
    .WithName("GetMedicalRecordByUuid")
    .WithOpenApi();


app.MapPut("medical-records/{uuid}", [Authorize(Roles = "Doctor")] async (string uuid, HttpRequest request, IEventRepository eventRepository) =>
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

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var questionnaireReceiver = scope.ServiceProvider.GetRequiredService<PatientQuestionnaireReceiver>();
    Task.Run(() => questionnaireReceiver.Receiver());
});

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var userReceiver = scope.ServiceProvider.GetRequiredService<IRabbitMqReceiverService>();
    Task.Run(() => userReceiver.Receiver());
});

app.Run();
