using Defender.Common.Configuration.Options;
using Defender.Common.DB.Model;
using Defender.Common.DB.Repositories;
using Defender.Common.DB.SharedStorage.Entities;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Transactions;
using Defender.RiskGamesService.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Defender.RiskGamesService.Infrastructure.Repositories.Transactions;

public class TransactionToTrackRepository(IOptions<MongoDbOptions> mongoOption)
    : BaseMongoRepository<TransactionToTrack>(mongoOption.Value), 
        ITransactionToTrackRepository
{
    public async Task<List<TransactionToTrack>> GetTransactionsAsync(
        string drawId,
        GameType gameType)
    {
        var filterRequest = FindModelRequest<TransactionToTrack>
            .Init(x => x.DrawId, drawId)
            .And(x => x.GameType, gameType);

        return [.. (await GetItemsAsync(filterRequest))];
    }

    public Task<TransactionToTrack> CreateTransactionAsync(TransactionToTrack newModel)
    {
        return AddItemAsync(newModel);
    }

    public Task<TransactionToTrack> GetTransactionAsync(string transactionId)
    {
        var filterRequest = FindModelRequest<TransactionToTrack>
            .Init(x => x.TransactionId, transactionId);

        return GetItemAsync(filterRequest);
    }

    public Task DeleteTransactonAsync(string transactionId)
    {
        var filter = FindModelRequest<TransactionToTrack>
            .Init(x => x.TransactionId, transactionId).BuildFilterDefinition();

        return _mongoCollection.DeleteOneAsync(filter);
    }
}
