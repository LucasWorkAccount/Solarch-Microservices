using PatientManagement;
using Notification_Sender.RabbitMqReceivers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IQuestionnaireSender, PatientQuestionnaire>();
builder.Services.AddSingleton<RabbitMqNotificationReceiver>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => { return Results.Ok("Notification sender is reachable!"); })
    .WithName("SendNotification")
    .WithOpenApi();

app.MapPost("/questionnaire/{uuid}",
    (string uuid, Questionnaire questionnaire, IQuestionnaireSender questionnaireSender) =>
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

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var userReceiver = scope.ServiceProvider.GetRequiredService<RabbitMqNotificationReceiver>();
    Task.Run(() => userReceiver.Receiver(
            "General-practitioner-results-exchange",
            "General-practitioner-results-route-key",
            "General-practitioner-results",
            "General practitioner results receiver App",
            "Sending following results to general practitioner via email: "
        )
    );
});

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var userReceiver = scope.ServiceProvider.GetRequiredService<RabbitMqNotificationReceiver>();
    Task.Run(() => userReceiver.Receiver(
            "Appointment-reminder-exchange",
            "Appointment-reminder-route-key",
            "Appointment-reminder",
            "Appointment reminder sender App",
            "Sending email reminder for appointment: "
        )
    );
});

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var userReceiver = scope.ServiceProvider.GetRequiredService<RabbitMqNotificationReceiver>();
    Task.Run(() => userReceiver.Receiver(
            "Patient-arrival-notification-exchange",
            "Patient-arrival-notification-route-key",
            "Patient-arrival-notification",
            "Patient arrival notification receiver App",
            "Sending email doctor arrival notification: "
        )
    );
});

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var userReceiver = scope.ServiceProvider.GetRequiredService<RabbitMqNotificationReceiver>();
    Task.Run(() => userReceiver.Receiver(
            "Patient-referral-notification-exchange",
            "Patient-referral-notification-route-key",
            "Patient-referral-notification",
            "Patient referral notification Receiver App",
            "Sending email with referral to patient: "
        )
    );
});


app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var userReceiver = scope.ServiceProvider.GetRequiredService<RabbitMqNotificationReceiver>();
    Task.Run(() => userReceiver.Receiver(
            "request-research-exchange",
            "request-research-route-key",
            "request-research",
            "Request research Receiver App",
            "Sending email with research request to laboratory: "
        )
    );
});


app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var userReceiver = scope.ServiceProvider.GetRequiredService<RabbitMqNotificationReceiver>();
    Task.Run(() => userReceiver.Receiver(
            "send-recipe-exchange",
            "send-recipe-route-key",
            "send-recipe",
            "Recipe sender App",
            "Sending email with following recipe to Pharmacy: "
        )
    );
});

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var userReceiver = scope.ServiceProvider.GetRequiredService<RabbitMqNotificationReceiver>();
    Task.Run(() => userReceiver.Receiver(
            "billable-action-exchange",
            "billable-action-route-key",
            "billable-action",
            "Billable action Receiver App",
            "Sending billable action to healthcare provider: "
        )
    );
});

app.Run();
