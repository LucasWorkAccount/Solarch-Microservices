using System.Text.Json;
using Patient_Transferral_System.Entities;
using User_Management.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRabbitMqSenderService, RabbitMqSenderSenderService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/patients", async (HttpContext httpContext, IRabbitMqSenderService rabbitMqService) =>
    {
        PatientDataReceiver patientDataReceiver = new PatientDataReceiver();
        List<TransferralPatient> patients = await patientDataReceiver.GetPatientDataFromLines();

        foreach (TransferralPatient patient in patients)
        {
            var newPatient = new
            {
                patient.FirstName,
                patient.LastName,
                patient.Address,
                patient.PhoneNumber
            };
            rabbitMqService.Send("Patient-Transferral-System", JsonSerializer.Serialize(newPatient));
        }

        return Results.Ok("patients");
})
.WithName("TransferPatientData")
.WithOpenApi();

app.Run();
