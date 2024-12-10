using Defender.Common.DB.SharedStorage.Entities;
using Defender.Common.DB.SharedStorage.Enums;
using Defender.Mongo.MessageBroker.Configuration.Subscribe;
using Defender.Mongo.MessageBroker.Interfaces.Topic;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Defender.RiskGamesService.Application.Services.Background;

public class TransactionStatusesListenerService(
        ILogger<TransactionStatusesListenerService> logger,
        IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ScanAndProcessOutboxTableAsync(stoppingToken);
    }

    private async Task ScanAndProcessOutboxTableAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var consumer = CreateTopicConsumer(scope);
                var subscribeOption = CreateSubscribeOptions(scope);

                logger.LogInformation("Ready to subscribe to outbox topic");
                await consumer.SubscribeTopicAsync(subscribeOption, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break; // Expected during shutdown
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error subscribing to topic: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private static ITopicConsumer<TransactionStatusUpdatedEvent> CreateTopicConsumer(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<ITopicConsumer<TransactionStatusUpdatedEvent>>();
    }

    private static SubscribeOptions<TransactionStatusUpdatedEvent> CreateSubscribeOptions(IServiceScope scope)
    {
        var transactionManagementService = scope.ServiceProvider.GetRequiredService<ITransactionManagementService>();

        var builder = Builders<TransactionStatusUpdatedEvent>.Filter;

        var filter = builder.Eq(x => x.TransactionPurpose, TransactionPurpose.Lottery) &
            (builder.Eq(x => x.TransactionType, TransactionType.Payment)
            | builder.Eq(x => x.TransactionType, TransactionType.Recharge));

        return SubscribeOptionsBuilder<TransactionStatusUpdatedEvent>
                    .Create()
                    .SetAction(transactionManagementService
                        .HandleTransactionStatusUpdatedEvent)
                    .SetFilter(filter)
                    .Build();
    }
}
