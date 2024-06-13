using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;

public interface ILotteryDrawRepository
{
    Task<PagedResult<LotteryDraw>> GetActiveLotteryDrawsAsync(PaginationRequest request);
    Task<LotteryDraw> GetLotteryDrawAsync(long drawNumber);
    Task<LotteryDraw> CreateLotteryDrawAsync(LotteryDraw lotteryDraw);
    Task ProcessLotteryDrawsAsync(
        Func<LotteryDraw, Task> processAction,
        CancellationToken cancellationToken = default);
    Task<LotteryDraw> UpdateLotteryDrawAsync(UpdateModelRequest<LotteryDraw> updateRequest);
}
