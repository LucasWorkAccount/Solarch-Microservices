﻿using System.Text.Json;
using System.Text.Json.Nodes;
using Appointment_Planner.Entities;
using Appointment_Planner.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Appointment_Planner.Endpoints;

public static class AppointmentsModule
{
    public static IEndpointRouteBuilder MapAppointmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/appointments",
                [Authorize(Roles = "Receptionist")]
                async (string? patientUuid, IAppointmentRepository appointmentRepository) =>
                {
                    var appointments =
                        await appointmentRepository.GetAppointments(patientUuid == null ? null : new Guid(patientUuid));

                    return Results.Json(new
                    {
                        status = 200,
                        message = "Appointments retrieved successfully!",
                        appointments
                    });
                })
            .WithName("GetAppointments")
            .WithOpenApi();

        endpoints.MapPost("/appointments",
                [Authorize(Roles = "Practitioner")]
                async (AppointmentEmail email, IAppointmentRepository appointmentRepository,
                    IRabbitMqSenderService senderService) =>
                {
                    var referral = Guid.NewGuid();
                    var message = new
                    {
                        referral,
                        email.Email
                    };

                    senderService.Send(
                        "Patient-referral-notification",
                        JsonSerializer.Serialize(message),
                        "Patient-referral-notification-route-key",
                        "Patient-referral-notification-exchange",
                        "Patient referral notification sender App"
                    );

                    await appointmentRepository.CreateAppointment(referral);
                    return Results.Json(new
                    {
                        status = 200,
                        message = "Referral created successfully!",
                        referral
                    });
                })
            .WithName("CreateAppointment")
            .WithOpenApi();


        endpoints.MapPost("/appointments/{referral}",
                [Authorize(Roles = "Patient")]
                async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository) =>
                {
                    using var reader = new StreamReader(request.Body);
                    var json = JsonNode.Parse(await reader.ReadToEndAsync());

                    var appointment = new Appointment(
                        new Guid(json!["patient"]!.ToString()),
                        new Guid(json["doctor"]!.ToString()),
                        DateTime.Parse(json!["datetime"]!.ToString()),
                        json["arrival"]!.ToString(),
                        new Guid(referral)
                    );

                    await appointmentRepository.EditAppointment(appointment);
                    return Results.Json(new
                    {
                        status = 200,
                        message = "Appointment planned successfully!",
                        appointment
                    });
                })
            .WithName("PlanAppointmentByReferral")
            .WithOpenApi();


        endpoints.MapPost("/appointments/followup/{referral}",
                [Authorize(Roles = "Receptionist")]
                async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository) =>
                {
                    using var reader = new StreamReader(request.Body);
                    var json = JsonNode.Parse(await reader.ReadToEndAsync());
                    var appointment = await appointmentRepository.GetAppointment(new Guid(referral));

                    if (appointment == null)
                    {
                        return Results.Json(new
                        {
                            status = 404,
                            message = "Could not find an appointment with the given referral!",
                            referral
                        });
                    }

                    var followupAppointment = new Appointment(
                        appointment.Patient,
                        appointment.Doctor,
                        DateTime.Parse(json!["datetime"]!.ToString()),
                        "NotYet",
                        Guid.NewGuid()
                    );

                    await appointmentRepository.CreateAppointment(followupAppointment.Referral);
                    await appointmentRepository.EditAppointment(followupAppointment);

                    return Results.Json(new
                    {
                        status = 200,
                        message = "Follow-up appointment planned successfully!",
                        followupAppointment
                    });
                })
            .WithName("PlanFollowupAppointmentByReferral")
            .WithOpenApi();


        endpoints.MapPut("/appointments/reschedule/{referral}",
                [Authorize(Roles = "Receptionist,Patient,Doctor")]
                async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository) =>
                {
                    using var reader = new StreamReader(request.Body);
                    var json = JsonNode.Parse(await reader.ReadToEndAsync());
                    var appointmentToEdit = await appointmentRepository.GetAppointment(new Guid(referral));

                    if (appointmentToEdit == null)
                    {
                        return Results.Json(new
                        {
                            status = 404,
                            message = "Could not find an appointment with the given referral!",
                            referral
                        });
                    }

                    appointmentToEdit.Datetime = DateTime.Parse(json!["datetime"]!.ToString());
                    await appointmentRepository.EditAppointment(appointmentToEdit);

                    return Results.Json(new
                    {
                        status = 200,
                        message = "Appointment rescheduled successfully!",
                        appointmentToEdit
                    });
                })
            .WithName("RescheduleAppointmentByReferral")
            .WithOpenApi();


        endpoints.MapPut("/appointments/arrival/{referral}",
                [Authorize(Roles = "Receptionist")]
                async (string referral, HttpRequest request, IAppointmentRepository appointmentRepository,
                    IRabbitMqSenderService senderService) =>
                {
                    using var reader = new StreamReader(request.Body);
                    var json = JsonNode.Parse(await reader.ReadToEndAsync());
                    var appointmentToEdit = await appointmentRepository.GetAppointment(new Guid(referral));

                    if (appointmentToEdit == null)
                    {
                        return Results.Json(new
                        {
                            status = 404,
                            message = "Could not find an appointment with the given referral!",
                            referral
                        });
                    }

                    var validEnum = Enum.TryParse(json!["arrival"]!.ToString(), out Arrival arrival);
                    if (!validEnum)
                    {
                        return Results.Json(new
                        {
                            status = 400,
                            message = "Invalid arrival status! Expected one of the following:",
                            validArrivalStatuses = Enum.GetNames(typeof(Arrival))
                        });
                    }

                    if (arrival != Arrival.NotYet && arrival != Arrival.NoShow)
                    {
                        var message = new
                        {
                            appointmentToEdit.Patient,
                            appointmentToEdit.Doctor,
                            referral = appointmentToEdit.Referral,
                            arrival = appointmentToEdit.Arrival,
                            appointmentTime = appointmentToEdit.Datetime.TimeOfDay.ToString()
                        };
                        senderService.Send(
                            "Patient-arrival-notification",
                            JsonSerializer.Serialize(message),
                            "Patient-arrival-notification-route-key",
                            "Patient-arrival-notification-exchange",
                            "Patient arrival notification sender App"
                        );
                    }

                    appointmentToEdit.Arrival = arrival.ToString();
                    await appointmentRepository.EditAppointment(appointmentToEdit);

                    return Results.Json(new
                    {
                        status = 200,
                        message = "Appointment arrival status updated successfully!",
                        appointmentToEdit
                    });
                })
            .WithName("SetAppointmentArrivalByReferral")
            .WithOpenApi();

        return endpoints;
    }
}
