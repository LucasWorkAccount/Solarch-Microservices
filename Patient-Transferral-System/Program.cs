using Patient_Transferral_System.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/patients", async (HttpContext httpContext) =>
    {
        PatientDataReceiver patientDataReceiver = new PatientDataReceiver();
        var patients = patientDataReceiver.GetPatientDataFromLines();
        return Results.Ok(patients);
})
.WithName("TransferPatientData")
.WithOpenApi();

app.Run();
