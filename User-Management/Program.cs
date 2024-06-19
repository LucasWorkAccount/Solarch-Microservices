using Microsoft.EntityFrameworkCore;
using User_Management.Model;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/users/{uuid}",
        async (string uuid, IUserRepository userRepository) =>
        {
            return Results.Ok(userRepository.FindUserByUuid(uuid));
        })
    .WithName("GetUserByUuid")
    .WithOpenApi();

app.MapPost("/register", async (RegisterUser user, IUserRepository userRepository) =>
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
            return Results.Json(new { token = JWTGenerator.Generate(user) });
        }
        catch
        {
            return Results.Conflict("Invalid data");
        }
    })
    .WithName("AddUser")
    .WithOpenApi();

app.MapPost("/login", async (RegisterUser user, IUserRepository userRepository) =>
    {
        try
        {
            var userDb = await userRepository.FindUserByEmail(user.Email);
            if (userDb == null)
            {
                return Results.NotFound("Email/password incorrect");
            }

            var passwordCorrect = BCrypt.Net.BCrypt.Verify(user.Password, user.Password);
            var jwt = new { token = JWTGenerator.Generate(user) };
            return passwordCorrect ? Results.Json(jwt): Results.NotFound("Email/password incorrect");
        }
        catch
        {
            return Results.BadRequest("Bad request");
        }
    })
    .WithName("Login")
    .WithOpenApi();

app.Run();
