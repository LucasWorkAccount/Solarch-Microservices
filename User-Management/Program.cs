using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using User_Management.Model;
using User_Management.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var objBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
IConfiguration configManager = objBuilder.Build();
var connection = configManager.GetConnectionString("user-management-db");

builder.Services.AddDbContext<UserManagementDbContext>(options => options.UseNpgsql(connection));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IRabbitMqSenderService, RabbitMqSenderSenderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/register", async (RegisterUser user, IUserRepository userRepository, IRabbitMqSenderService rabbitMqService) =>
    {
        try
        {
            if (await userRepository.UserExists(user.Email))
            {
                return Results.Conflict("Email already used");
            }

            user.Uuid = Guid.NewGuid();
            Email.Validate(user.Email);
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await userRepository.AddUser(user);
            if (user.Role == "patient")
            {
                var MRSUser = new
                {
                    uuid = user.Uuid,
                    name = user.Name,
                    age = user.Age,
                    sex = user.Sex,
                    bsn = user.Bsn
                };
                rabbitMqService.Send("Medical-record-system-register", JsonSerializer.Serialize(MRSUser));
            }

            return Results.Json(new { token = JWTGenerator.Generate(user) });
        }
        catch(Exception e)
        {
            return Results.Conflict(e.Message);
        }
    })
    .WithName("AddUser")
    .WithOpenApi();

app.MapPost("/login", async (LoginUser user, IUserRepository userRepository) =>
    {
        try
        {
            var userDb = await userRepository.FindUserByEmail(user.Email);
            if (userDb == null)
            {
                return Results.NotFound("Email/password incorrect");
            }

            var passwordCorrect = BCrypt.Net.BCrypt.Verify(user.Password, userDb.Password);
            var jwt = new { token = JWTGenerator.Generate(userDb) };
            return passwordCorrect ? Results.Json(jwt) : Results.NotFound("Email/password incorrect");
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    })
    .WithName("Login")
    .WithOpenApi();

app.Run();
