using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Domain.Entities.Lottery;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;

public interface ILotteryRepository
{
    Task<List<LotteryModel>> GetAllLotteriesToScheduleAsync();
    Task<PagedResult<LotteryModel>> GetLotteriesAsync(
        PaginationRequest pagination, string? name);
    Task<LotteryModel> GetLotteryModelByIdAsync(Guid id);
    Task<LotteryModel> CreateNewLotteryAsync(LotteryModel newLottery);
    Task<LotteryModel> UpdateLotteryAsync(UpdateModelRequest<LotteryModel> request);
    Task DeleteLotteryAsync(Guid lotteryId);
}
