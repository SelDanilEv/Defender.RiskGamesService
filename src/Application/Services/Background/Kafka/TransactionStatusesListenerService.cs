using Defender.Common.DB.SharedStorage.Entities;
using Defender.Common.DB.SharedStorage.Enums;
using Defender.Common.Kafka;
using Defender.Common.Kafka.Default;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Defender.RiskGamesService.Application.Services.Background.Kafka;

public class TransactionStatusesListenerService(
    IDefaultKafkaConsumer<TransactionStatusUpdatedEvent> updatedStatusesConsumer,
    ILogger<TransactionStatusesListenerService> logger,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    private readonly Lazy<ITransactionManagementService> _transactionManagementService = new(() =>
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ITransactionManagementService>();
    });

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10_000, stoppingToken);
        
        await updatedStatusesConsumer.StartConsuming(
            Topic.TransactionStatusUpdates.GetName(),
            HandleTransactionStatusUpdatedEvent,
            stoppingToken);
    }

    private Task HandleTransactionStatusUpdatedEvent(
        TransactionStatusUpdatedEvent transactionStatusUpdatedEvent)
    {
        if (transactionStatusUpdatedEvent is
            {
                TransactionPurpose: TransactionPurpose.Lottery,
                TransactionType: TransactionType.Payment or TransactionType.Recharge
            })
        {
            return _transactionManagementService.Value
                .HandleTransactionStatusUpdatedEvent(transactionStatusUpdatedEvent);
        }

        return Task.CompletedTask;
    }
}