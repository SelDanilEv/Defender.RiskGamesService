using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Defender.RiskGamesService.Application.Services.Lottery.Background;

public class LotteryScheduleAndProcessingService : BackgroundService
{
    private readonly ILotteryProcessingService _lotteryProcessingService;
    private readonly ILogger<LotteryScheduleAndProcessingService> _logger;

    public LotteryScheduleAndProcessingService(ILotteryProcessingService lotteryProcessingService, ILogger<LotteryScheduleAndProcessingService> logger)
    {
        _lotteryProcessingService = lotteryProcessingService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var timer = Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            try
            {
                await _lotteryProcessingService.ScanAndProcessLotteries();
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing lottery draws");
            }
            finally
            {
                await timer;
            }
        }
    }
}
