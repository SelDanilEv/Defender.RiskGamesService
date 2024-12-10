using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;

public interface ILotteryDrawRepository
{
    Task<PagedResult<LotteryDraw>> GetActiveLotteryDrawsAsync(PaginationRequest request);
    Task<LotteryDraw> GetLotteryDrawAsync(Guid drawId);
    Task<LotteryDraw> GetLotteryDrawAsync(long drawNumber);
    Task<LotteryDraw> CreateLotteryDrawAsync(LotteryDraw lotteryDraw);
    Task<List<Guid>> GetLotteryDrawsToProcessAsync(
        CancellationToken cancellationToken = default);
    Task MarkDrawAsProcessedAsync(Guid drawId);
    Task<LotteryDraw> UpdateLotteryDrawAsync(UpdateModelRequest<LotteryDraw> updateRequest);
}
