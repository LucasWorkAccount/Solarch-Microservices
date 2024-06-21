using PatientManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IQuestionnaireSender, PatientQuestionnaire>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
    {
        return Results.Ok("Notification sender is reachable!");
    })
    .WithName("SendNotification")
    .WithOpenApi();

app.MapPost("/questionnaire", (Questionnaire questionnaire, IQuestionnaireSender questionnaireSender) =>
{
    try
    {
        questionnaireSender.Send("PatientQuestionnaireQueue", questionnaire);
    }
    catch (Exception exception)
    {
        Console.WriteLine(exception.Message);
    }
});

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var patientQuestionnaire = services.GetRequiredService<PatientQuestionnaire>();
//     patientQuestionnaire.Send();
// }

app.Run();