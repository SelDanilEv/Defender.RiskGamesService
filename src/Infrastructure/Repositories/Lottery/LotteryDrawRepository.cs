using Defender.Common.Configuration.Options;
using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.Common.DB.Repositories;
using Defender.Common.Exceptions;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Defender.RiskGamesService.Infrastructure.Repositories.Lottery;

public class LotteryDrawRepository : BaseMongoRepository<LotteryDraw>, ILotteryDrawRepository
{
    public LotteryDrawRepository(IOptions<MongoDbOptions> mongoOption) : base(mongoOption.Value)
    {
        _mongoCollection.Indexes.CreateMany([
            new CreateIndexModel<LotteryDraw>(
                Builders<LotteryDraw>.IndexKeys.Ascending(x => x.DrawNumber),
                new CreateIndexOptions { Unique = true, Name = "DrawNumber_Index" }),
            new CreateIndexModel<LotteryDraw>(
                Builders<LotteryDraw>.IndexKeys
                    .Descending(x => x.EndDate)
                    .Descending(x => x.IsProcessed)
                    .Descending(x => x.IsProcessing),
                new CreateIndexOptions { Name = "EndDate_IsProcessed_IsProcessing" })
        ]);
    }

    public Task<PagedResult<LotteryDraw>> GetActiveLotteryDrawsAsync(PaginationRequest request)
    {
        var filter = FindModelRequest<LotteryDraw>
            .Init(x => x.EndDate, DateTime.UtcNow.AddSeconds(30), FilterType.Gt)
            .Sort(x => x.EndDate, SortType.Asc);

        var settings = PaginationSettings<LotteryDraw>.FromPaginationRequest(request);

        settings.SetupFindOptions(filter);

        return GetItemsAsync(settings);
    }

    public Task<LotteryDraw> GetLotteryDrawAsync(Guid drawId)
    {
        return GetItemAsync(drawId);
    }

    public Task<LotteryDraw> GetLotteryDrawAsync(long drawNumber)
    {
        var filter = FindModelRequest<LotteryDraw>.Init(x => x.DrawNumber, drawNumber);

        return GetItemAsync(filter);
    }

    public async Task<LotteryDraw> CreateLotteryDrawAsync(LotteryDraw lotteryDraw)
    {
        bool isUnique;
        do
        {
            lotteryDraw.DrawNumber = await GetNextDrawNumber();
            try
            {
                await AddItemAsync(lotteryDraw);
                isUnique = true;
            }
            catch (ServiceException ex) when (
                ex.InnerException is MongoWriteException ex2 &&
                ex2.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                isUnique = false;
            }
        }
        while (!isUnique);

        return lotteryDraw;
    }

    public async Task<List<Guid>> GetLotteryDrawsToProcessAsync(
        CancellationToken cancellationToken = default)
    {
        var options = new FindOptions<LotteryDraw>
        {
            CursorType = CursorType.NonTailable
        };

        var findRequest = FindModelRequest<LotteryDraw>
            .Init(x => x.EndDate, DateTime.UtcNow, FilterType.Lte)
            .And(x => x.IsProcessed, false)
            .And(x => x.IsProcessing, false);

        using var cursor = await _mongoCollection.FindAsync(
            findRequest.BuildFilterDefinition(),
            options,
            cancellationToken);

        var result = new List<Guid>();

        await cursor.ForEachAsync(async document =>
        {
            findRequest = FindModelRequest<LotteryDraw>
                .Init(x => x.Id, document.Id)
                .And(x => x.IsProcessed, false)
                .And(x => x.IsProcessing, false);

            var update = Builders<LotteryDraw>.Update.Set(x => x.IsProcessing, true);
            var findAndUpdateOptions = new FindOneAndUpdateOptions<LotteryDraw>
            {
                ReturnDocument = ReturnDocument.After
            };

            document = await _mongoCollection.FindOneAndUpdateAsync(
                findRequest.BuildFilterDefinition(),
                update,
                findAndUpdateOptions,
                cancellationToken);

            if (document == null)
            {
                // Another service is already processing this document
                return;
            }

            result.Add(document.Id);
        }, cancellationToken: cancellationToken);

        return result;
    }

    public Task MarkDrawAsProcessedAsync(Guid drawId)
    {
        var filter = Builders<LotteryDraw>.Filter.Eq(x => x.Id, drawId);

        var update = Builders<LotteryDraw>.Update
            .Set(x => x.IsProcessing, false)
            .Set(x => x.IsProcessed, true);

        return _mongoCollection.UpdateOneAsync(
            filter,
            update);
    }

    public async Task<LotteryDraw> UpdateLotteryDrawAsync(
        UpdateModelRequest<LotteryDraw> updateRequest)
    {
        return await UpdateItemAsync(updateRequest);
    }

    private async Task<long> GetNextDrawNumber()
    {
        var highestDrawNumber = await GetItemAsync(
            FindModelRequest<LotteryDraw>.Init()
                .Sort(x => x.DrawNumber, SortType.Desc));

        return highestDrawNumber is null ? 0 : ++highestDrawNumber.DrawNumber;
    }
}
