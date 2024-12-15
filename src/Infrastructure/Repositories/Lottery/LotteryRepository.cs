using Defender.Common.Configuration.Options;
using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.Common.DB.Repositories;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery;
using Microsoft.Extensions.Options;

namespace Defender.RiskGamesService.Infrastructure.Repositories.Lottery;

public class LotteryRepository(IOptions<MongoDbOptions> mongoOption)
    : BaseMongoRepository<LotteryModel>(mongoOption.Value), ILotteryRepository
{
    public async Task<List<LotteryModel>> GetAllLotteriesToScheduleAsync()
    {
        var filterRequest = FindModelRequest<LotteryModel>
            .Init(x => x.IsActive, true)
            .And(x => x.IncomePercentage, 0, FilterType.Gt)
            .And(x => x.Schedule!.NextStartDate, DateTime.UtcNow, FilterType.Lte);

        return [.. (await GetItemsAsync(filterRequest))];
    }

    public Task<PagedResult<LotteryModel>> GetLotteriesAsync(
        PaginationRequest request, string? name)
    {
        var settings = PaginationSettings<LotteryModel>
            .FromPaginationRequest(request);

        if (!string.IsNullOrWhiteSpace(name))
        {
            var filterRequest = FindModelRequest<LotteryModel>
                .Init(x => x.Name, name)
                .Sort(x => x.CreatedDate, SortType.Desc);

            settings.SetupFindOptions(filterRequest);
        }

        return GetItemsAsync(settings);
    }

    public Task<LotteryModel> GetLotteryModelByIdAsync(Guid id)
    {
        return GetItemAsync(id);
    }

    public Task<LotteryModel> CreateNewLotteryAsync(LotteryModel newLottery)
    {
        newLottery.CreatedDate = DateTime.UtcNow;
        newLottery.UpdatedDate = DateTime.UtcNow;
        newLottery.IsActive = false;

        return AddItemAsync(newLottery);
    }

    public Task<LotteryModel> UpdateLotteryAsync(UpdateModelRequest<LotteryModel> request)
    {
        request.SetIfNotNull(x => x.UpdatedDate, DateTime.UtcNow);

        return UpdateItemAsync(request);
    }

    public Task<LotteryModel> UpdateLotteryAsync(
        UpdateModelRequest<LotteryModel> request,
        FindModelRequest<LotteryModel> filter)
    {
        request.SetIfNotNull(x => x.UpdatedDate, DateTime.UtcNow);

        return UpdateItemAsync(request, filter);
    }

    public Task DeleteLotteryAsync(Guid lotteryId)
    {
        return RemoveItemAsync(lotteryId);
    }
}
