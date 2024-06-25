using System.Text;
using Appointment_Planner;
using Appointment_Planner.Endpoints;
using Appointment_Planner.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddSingleton<IRabbitMqSenderService, RabbitMqSenderService>();
builder.Services.AddSingleton<IAppointmentReminderSender, AppointmentReminderSender>();

builder.Services.AddHostedService<CronJobService>(provider =>
{
    var sender = provider.GetRequiredService<IAppointmentReminderSender>();
    // Set every minute for testing example, for daily reminders at 18:00 use "0 18 * * *"
    const string cronExpression = "* * * * *";
    return new CronJobService(cronExpression, sender);
});

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

app.MapResearchEndpoints();
app.MapAppointmentEndpoints();

app.Run();
