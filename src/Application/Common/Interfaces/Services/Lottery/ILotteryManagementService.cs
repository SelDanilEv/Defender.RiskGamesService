using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Application.Models.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;

public interface ILotteryManagementService
{
    Task<PagedResult<LotteryModel>> GetLotteriesAsync(PaginationRequest request, string? name = "");
    Task<LotteryModel> CreateLotteryAsync(CreateLotteryRequest createRequest);
    Task<LotteryModel> UpdateLotteryAsync(UpdateLotteryRequest updateRequest);
    Task<LotteryModel> GetLotteryAsync(Guid lotteryId);
    Task ActivateLotteryAsync(Guid lotteryId);
    Task DeactivateLotteryAsync(Guid lotteryId);
    Task DeleteLotteryAsync(Guid lotteryId);

    Task ScheduleDraws();
    Task<LotteryDraw> GetLotteryDrawByNumberAsync(long drawNumber);
    Task<PagedResult<LotteryDraw>> GetActiveDrawAsync(PaginationRequest request);

}
