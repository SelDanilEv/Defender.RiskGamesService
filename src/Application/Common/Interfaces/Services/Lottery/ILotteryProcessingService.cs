namespace Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;

public interface ILotteryProcessingService
{
    Task QueueLotteriesForProcessing(CancellationToken cancellationToken = default);
    Task HandleLotteryDraw(Guid drawId);
}
