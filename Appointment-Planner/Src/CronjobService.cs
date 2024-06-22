using Appointment_Planner.Repositories;

namespace Appointment_Planner;

using Cronos;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CronJobService : BackgroundService
{
    private readonly IAppointmentReminderSender _sender;

    private readonly string _cronExpression;
    private readonly CronExpression _cron;
    private DateTime _nextRun;

    public CronJobService(string cronExpression, IAppointmentReminderSender sender)
    {
        _cronExpression = cronExpression;
        _sender = sender;
        _cron = CronExpression.Parse(_cronExpression);
        _nextRun = DateTime.UtcNow;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            if (now > _nextRun)
            {
                await DoWork(stoppingToken);
                _nextRun = _cron.GetNextOccurrence(now, TimeZoneInfo.Utc) ?? DateTime.UtcNow.AddSeconds(30);
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Delay until next check
        }
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        try
        {
            _sender.Send("Appointment-reminder");
            Console.WriteLine("Task running at: " + DateTime.UtcNow);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        await Task.CompletedTask;
    }
}
