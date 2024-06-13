namespace Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;

public interface ILotteryProcessingService
{
    Task ScanAndProcessLotteries();
}
