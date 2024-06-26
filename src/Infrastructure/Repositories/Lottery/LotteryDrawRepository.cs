﻿using Defender.Common.Configuration.Options;
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
        _mongoCollection.Indexes.CreateOne(new CreateIndexModel<LotteryDraw>(
            Builders<LotteryDraw>.IndexKeys.Ascending(x => x.DrawNumber),
            new CreateIndexOptions { Unique = true }));
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

    public async Task<LotteryDraw> GetLotteryDrawAsync(long drawNumber)
    {
        var filter = FindModelRequest<LotteryDraw>.Init(x => x.DrawNumber, drawNumber);

        return await GetItemAsync(filter);
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

    public async Task ProcessLotteryDrawsAsync(
        Func<LotteryDraw, Task> processAction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(processAction);

        var options = new FindOptions<LotteryDraw>
        {
            CursorType = CursorType.NonTailable
        };

        var findRequest = FindModelRequest<LotteryDraw>
            .Init(x => x.EndDate, DateTime.UtcNow, FilterType.Lte)
            .And(x => x.IsProcessed, false);

        using var cursor = await _mongoCollection.FindAsync(
            findRequest.BuildFilterDefinition(),
            options,
            cancellationToken);

        await cursor.ForEachAsync(async document =>
        {
            var filter = Builders<LotteryDraw>.Filter.Eq(x => x.Id, document.Id);
            var update = Builders<LotteryDraw>.Update.Set(x => x.IsProcessing, true);
            var options = new FindOneAndUpdateOptions<LotteryDraw>
            {
                ReturnDocument = ReturnDocument.After
            };

            document = await _mongoCollection.FindOneAndUpdateAsync(
                filter,
                update,
                options,
                cancellationToken);

            if (document == null)
            {
                // Another service is already processing this document
                return;
            }

            await processAction(document);

            update = Builders<LotteryDraw>.Update
                .Set(x => x.IsProcessing, false)
                .Set(x => x.IsProcessed, true);

            await _mongoCollection.UpdateOneAsync(
                filter,
                update);
        }, cancellationToken: cancellationToken);

    }

    public async Task<LotteryDraw> UpdateLotteryDrawAsync(
        UpdateModelRequest<LotteryDraw> updateRequest)
    {
        return await UpdateItemAsync(updateRequest);
    }

    private async Task<long> GetNextDrawNumber()
    {
        var highestDrawNumber = await CountItemsAsync();

        return highestDrawNumber;
    }

    private async Task<bool> EnsureUniqueness(long drawNumber)
    {
        var highestDrawNumber = await CountItemsAsync(
            FindModelRequest<LotteryDraw>.Init(x => x.DrawNumber, drawNumber));

        return highestDrawNumber == 0;
    }
}
