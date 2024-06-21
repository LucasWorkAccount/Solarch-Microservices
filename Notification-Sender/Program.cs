using Notification_Sender.RabbitMqReceivers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<GeneralPractitionerResultsReceiver>();

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

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var userReceiver = scope.ServiceProvider.GetRequiredService<GeneralPractitionerResultsReceiver>();
    Task.Run(() => userReceiver.Receiver());
});

app.Run();
