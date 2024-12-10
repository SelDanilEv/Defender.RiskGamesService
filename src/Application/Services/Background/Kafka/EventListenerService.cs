using Defender.Common.Extension;
using Defender.Common.Kafka.Default;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Defender.RiskGamesService.Application.Services.Background.Kafka;

public class EventListenerService(
    IHostEnvironment hostEnvironment,
    IDefaultKafkaConsumer<string> kafkaStringEventConsumer,
    IDefaultKafkaConsumer<Guid> lotteriesToProceedConsumer,
    ILotteryProcessingService lotteryProcessingService,
    ILotteryManagementService lotteryManagementService,
    ILogger<EventListenerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10_000, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.WhenAll(
                    kafkaStringEventConsumer.StartConsuming(
                        KafkaTopic.ScheduledTasks.GetName(hostEnvironment.GetAppEnvironment()),
                        HandleStringEvent,
                        stoppingToken),
                    lotteriesToProceedConsumer.StartConsuming(
                        KafkaTopic.LotteryToProcess.GetName(hostEnvironment.GetAppEnvironment()),
                        lotteryProcessingService.HandleLotteryDraw,
                        stoppingToken)
                );
            }
            finally
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private Task HandleStringEvent(string @event)
    {
        switch (@event.ToEvent())
        {
            case KafkaEvent.StartLotteriesProcessing:
                _ = lotteryProcessingService.QueueLotteriesForProcessing();
                break;
            case KafkaEvent.ScheduleNewLotteryDraws:
                _ = lotteryManagementService.ScheduleDraws();
                break;
            default:
                logger.LogWarning("Unknown event: {0}", @event);
                break;
        }

        return Task.CompletedTask;
    }
}