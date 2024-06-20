using Defender.Common.Configuration.Options;
using Defender.Common.DB.Model;
using Defender.Common.DB.Repositories;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Transactions;
using Defender.RiskGamesService.Domain.Entities.Transactions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Defender.RiskGamesService.Infrastructure.Repositories.Transactions;

public class OutboxTransactionStatusRepository(IOptions<MongoDbOptions> mongoOption)
    : BaseMongoRepository<OutboxTransactionStatus>(mongoOption.Value),
        IOutboxTransactionStatusRepository
{
    public async Task<bool> TryLockTransactionStatusToHandleAsync(
        Guid id)
    {
        var findRequest = FindModelRequest<OutboxTransactionStatus>
            .Init(x => x.Id, id)
            .And(x => x.HandlerId, Guid.Empty);

        var handlerId = Guid.NewGuid();

        var updateRequest = UpdateModelRequest<OutboxTransactionStatus>
            .Init()
            .Set(x => x.HandlerId, handlerId);

        var options = new FindOneAndUpdateOptions<OutboxTransactionStatus>
        { ReturnDocument = ReturnDocument.After };

        var result = await _mongoCollection
            .FindOneAndUpdateAsync(
                findRequest.BuildFilterDefinition(),
                updateRequest.BuildUpdateDefinition(),
                options);

        if (result is null || result.HandlerId != handlerId)
        {
            // Another process is already processing this document
            return false;
        }

        return true;
    }

    public Task ReleaseTransactionStatusAsync(
        Guid id)
    {
        var findRequest = FindModelRequest<OutboxTransactionStatus>
            .Init(x => x.Id, id)
            .And(x => x.HandlerId, Guid.Empty, FilterType.Ne);

        var updateRequest = UpdateModelRequest<OutboxTransactionStatus>
            .Init()
            .Set(x => x.HandlerId, Guid.Empty);

        return _mongoCollection
            .UpdateOneAsync(
                findRequest.BuildFilterDefinition(),
                updateRequest.BuildUpdateDefinition());
    }

    public Task<OutboxTransactionStatus> CreateTransactionStatusAsync(
        OutboxTransactionStatus newModel)
    {
        return AddItemAsync(newModel);
    }

    public async Task<List<OutboxTransactionStatus>> GetAllAsync()
    {
        return [.. (await GetItemsAsync())];
    }

    public Task<OutboxTransactionStatus> IncreaseAttemptCountAsync(OutboxTransactionStatus transaction)
    {
        var updateRequest = UpdateModelRequest<OutboxTransactionStatus>
            .Init(transaction.Id)
            .Set(x => x.Attempt, ++transaction.Attempt);

        return UpdateItemAsync(updateRequest);
    }

    public Task DeleteTransactonStatusAsync(Guid id)
    {
        return RemoveItemAsync(id);
    }
}
