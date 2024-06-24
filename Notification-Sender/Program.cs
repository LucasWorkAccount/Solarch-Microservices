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

app.MapPost("/questionnaire/{uuid}", (string uuid, Questionnaire questionnaire, IQuestionnaireSender questionnaireSender) =>
{
    try
    {
        Console.WriteLine("Going to send!");
        questionnaire.uuid = new Guid(uuid);
        questionnaireSender.Send("PatientQuestionnaireQueue", questionnaire);
    }
    catch (Exception exception)
    {
        Console.WriteLine(exception.Message);
    }

    return Results.Ok("questionnaire send successfully");
});

app.Run();