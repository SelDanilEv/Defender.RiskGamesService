using Defender.Common.DB.SharedStorage.Entities;
using Defender.Common.DB.SharedStorage.Enums;
using Defender.Mongo.MessageBroker.Configuration;
using Defender.Mongo.MessageBroker.Interfaces.Topic;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Defender.RiskGamesService.Application.Services.Background;

public class TransactionStatusesListenerService(
        ITopicConsumer<TransactionStatusUpdatedEvent> consumer,
        ITransactionManagementService transactionManagementService)
    : BackgroundService, IDisposable
{
    private const int RunEachMinute = 1;
    private Timer? _timer;
    private bool _isRunning = false;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(async _ => await Retry(null, stoppingToken),
            null, TimeSpan.FromMinutes(RunEachMinute), TimeSpan.FromMinutes(RunEachMinute));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var builder = Builders<TransactionStatusUpdatedEvent>.Filter;
                var filter = builder.Eq(x => x.TransactionPurpose, TransactionPurpose.Lottery) &
                    (builder.Eq(x => x.TransactionType, TransactionType.Payment)
                        | builder.Eq(x => x.TransactionType, TransactionType.Recharge));

                var options = SubscribeOptionsBuilder<TransactionStatusUpdatedEvent>
                    .Create()
                    .SetAction(transactionManagementService
                        .HandleTransactionStatusUpdatedEvent)
                    .SetFilter(filter)
                    .Build();

                await consumer.SubscribeTopicAsync(
                    options,
                    stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }

    private async Task Retry(object? state, CancellationToken stoppingToken)
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        try
        {
            await transactionManagementService.ScanAndProccessOutboxTableAsync();
        }
        finally
        {
            _isRunning = false;
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
    }
}
