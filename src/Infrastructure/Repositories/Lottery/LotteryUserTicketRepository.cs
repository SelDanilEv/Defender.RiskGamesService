using Defender.Common.Configuration.Options;
using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.Common.DB.Repositories;
using Defender.Common.Errors;
using Defender.Common.Exceptions;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;
using Microsoft.Extensions.Options;

namespace Defender.RiskGamesService.Infrastructure.Repositories.Lottery;

public class LotteryUserTicketRepository(IOptions<MongoDbOptions> mongoOption)
    : BaseMongoRepository<UserTicket>(mongoOption.Value), ILotteryUserTicketRepository
{
    public Task<PagedResult<UserTicket>> GetUserTicketsByUserIdAsync(
        PaginationRequest request, Guid userId)
    {
        var findRequest = FindModelRequest<UserTicket>
            .Init(x => x.UserId, userId)
            .Sort(x => x.PurchaseDate, SortType.Desc);

        var settings = PaginationSettings<UserTicket>.FromPaginationRequest(request);

        settings.SetupFindOptions(findRequest);

        return GetItemsAsync(settings);
    }

    public async Task<List<UserTicket>> GetUserTicketsByDrawNumberAsync(long drawNumber)
    {
        var filterRequest = FindModelRequest<UserTicket>
            .Init(x => x.DrawNumber, drawNumber);

        var items = await GetItemsAsync(filterRequest);

        return [.. items];
    }

    public async Task<List<UserTicket>> CreateUserTicketsAsync(List<UserTicket> newModels)
    {
        newModels.ForEach(userTicket =>
        {
            userTicket.PurchaseDate = DateTime.UtcNow;
            userTicket.Status = UserTicketStatus.Requested;
        });

        try
        {
            await _mongoCollection.InsertManyAsync(newModels);
        }
        catch (Exception ex)
        {
            throw new ServiceException(ErrorCode.CM_DatabaseIssue, ex);
        }

        return newModels;
    }

    public async Task<UserTicket> UpdateUserTicketAsync(
        UpdateModelRequest<UserTicket> request)
    {
        return await UpdateItemAsync(request);
    }

    public async Task<long> UpdateManyUserTicketsAsync(
        FindModelRequest<UserTicket> findRequest,
        UpdateModelRequest<UserTicket> request)
    {
        var result = await _mongoCollection.UpdateManyAsync(
            findRequest.BuildFilterDefinition(),
            request.BuildUpdateDefinition());

        return result.ModifiedCount;
    }

    public Task DeleteTicketByPaymentTransactionIdAsync(string transactionId)
    {
        var filter = FindModelRequest<UserTicket>
            .Init(x => x.PaymentTransactionId, transactionId).BuildFilterDefinition();

        return _mongoCollection.DeleteOneAsync(filter);
    }
}
