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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/users/{uuid}", async (string uuid, UserManagementDbContext userManagementDbContext) =>
    {
        var user = await userManagementDbContext.Users.FirstOrDefaultAsync(user => user.Uuid == new Guid(uuid));
        return Results.Ok(user);
    })
    .WithName("GetUserByUuid")
    .WithOpenApi();

app.Run();
