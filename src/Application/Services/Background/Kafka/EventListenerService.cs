﻿using Defender.Kafka.Default;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Common.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Defender.RiskGamesService.Application.Services.Background.Kafka;

public class EventListenerService(
    IDefaultKafkaConsumer<string> kafkaStringEventConsumer,
    IDefaultKafkaConsumer<Guid> lotteriesToProceedConsumer,
    ILotteryProcessingService lotteryProcessingService,
    ILotteryManagementService lotteryManagementService,
    ILogger<EventListenerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10_000, stoppingToken);

        await Task.WhenAll(
            kafkaStringEventConsumer.StartConsuming(
                KafkaTopic.ScheduledTasks.GetName(),
                HandleStringEvent,
                stoppingToken),
            lotteriesToProceedConsumer.StartConsuming(
                KafkaTopic.LotteryToProcess.GetName(),
                lotteryProcessingService.HandleLotteryDraw,
                stoppingToken)
        );
    }

    private async Task HandleStringEvent(string @event)
    {
        try
        {
            logger.LogInformation("Incoming event: {0}", @event);

            switch (@event.ToEvent())
            {
                case KafkaEvent.StartLotteriesProcessing:
                    await lotteryProcessingService.QueueLotteriesForProcessing();
                    break;
                case KafkaEvent.ScheduleNewLotteryDraws:
                    await lotteryManagementService.ScheduleDraws();
                    break;
                default:
                    logger.LogWarning("Unknown event: {0}", @event);
                    break;
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error while handling event: {0}", @event);
        }
    }
}