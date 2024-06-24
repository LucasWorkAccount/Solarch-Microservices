using System.Text.Json;
using Appointment_Planner.Entities;
using Appointment_Planner.Repositories;

namespace Appointment_Planner.Endpoints;

public static class ResearchesModule
{
    public static IEndpointRouteBuilder MapResearchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/research-results",
                (ResearchResults researchResults, IRabbitMqSenderService senderService) =>
                {
                    try
                    {
                        senderService.Send(
                            "General-practitioner-results",
                            JsonSerializer.Serialize(researchResults),
                            "General-practitioner-results-route-key",
                            "General-practitioner-results-exchange",
                            "General practitioner results receiver App"
                        );

                        return Results.Json(new
                        {
                            status = 200,
                            message = "Email sent to general practitioner successfully!"
                        });
                    }
                    catch
                    {
                        return Results.Json(new
                        {
                            status = 400,
                            message = "Failed to send email to general practitioner!"
                        });
                    }
                })
            .WithName("SendResearchResults")
            .WithOpenApi();

        return endpoints;
    }
}
