using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Microsoft.Extensions.Hosting;

namespace Defender.RiskGamesService.Application.Services.Lottery.Background;

public class LotteryScheduleAndProcessingService(
        ILotteryProcessingService lotteryProcessingService)
    : IHostedService, IDisposable
{
    private Timer? _timer;
    private bool _isRunning = false;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(async _ => await Retry(null, cancellationToken),
            null, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    private async Task Retry(object? state, CancellationToken stoppingToken)
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;

        if (!stoppingToken.IsCancellationRequested)
        {
            await lotteryProcessingService.ScanAndProcessLotteries();
        }

        _isRunning = false;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
