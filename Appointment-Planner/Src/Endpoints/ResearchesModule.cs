using System.Text.Json;
using Appointment_Planner.Entities;
using Appointment_Planner.Repositories;

namespace Appointment_Planner.Endpoints;

public static class ResearchesModule
{
    public static IEndpointRouteBuilder MapResearchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/research-results-to-practitioner",
                (ResearchResults researchResults, IRabbitMqSenderService senderService) =>
                {
                    try
                    {
                        senderService.Send(
                            "General-practitioner-results",
                            JsonSerializer.Serialize(researchResults),
                            "General-practitioner-results-route-key",
                            "General-practitioner-results-exchange",
                            "General practitioner results sender App"
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
            .WithName("SendResearchResultsToGeneralPractitioner")
            .WithOpenApi();


        endpoints.MapPost("/request-research",
                (ResearchRequest researchRequest, IRabbitMqSenderService senderService) =>
                {
                    try
                    {
                        senderService.Send(
                            "request-research",
                            JsonSerializer.Serialize(researchRequest),
                            "request-research-route-key",
                            "request-research-exchange",
                            "Request research sender App"
                        );

                        senderService.Send(
                            "billable-action",
                            JsonSerializer.Serialize(researchRequest),
                            "billable-action-route-key",
                            "billable-action-exchange",
                            "billable action sender App"
                        );

                        return Results.Json(new
                        {
                            status = 200,
                            message = "Email with research request sent to research laboratory"
                        });
                    }
                    catch
                    {
                        return Results.Json(new
                        {
                            status = 400,
                            message = "Failed to send research"
                        });
                    }
                })
            .WithName("RequestResearch")
            .WithOpenApi();

        endpoints.MapPost("/send-recipe",
                (RecipeRequest recipeRequest, IRabbitMqSenderService senderService) =>
                {
                    try
                    {
                        senderService.Send(
                            "send-recipe",
                            JsonSerializer.Serialize(recipeRequest),
                            "send-recipe-route-key",
                            "send-recipe-exchange",
                            "recipe sender App"
                        );

                        senderService.Send(
                            "billable-action",
                            JsonSerializer.Serialize(recipeRequest),
                            "billable-action-route-key",
                            "billable-action-exchange",
                            "billable action sender App"
                        );

                        return Results.Json(new
                        {
                            status = 200,
                            message = "Email with Recipe sent to pharmacy"
                        });
                    }
                    catch
                    {
                        return Results.Json(new
                        {
                            status = 400,
                            message = "Failed to send recipe"
                        });
                    }
                })
            .WithName("SendRecipe")
            .WithOpenApi();


        return endpoints;
    }
}
