using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;

public interface ILotteryUserTicketRepository
{
    Task<PagedResult<UserTicket>> GetUserTicketsByUserIdAsync(PaginationRequest request, Guid userId);

    Task<List<UserTicket>> CreateUserTicketsAsync(List<UserTicket> newModels);

    Task<List<UserTicket>> GetUserTicketsByDrawNumberAsync(long drawNumber);

    Task<UserTicket> UpdateUserTicketAsync(UpdateModelRequest<UserTicket> request);

    Task<long> UpdateManyUserTicketsAsync(
        FindModelRequest<UserTicket> findRequest,
        UpdateModelRequest<UserTicket> request);

    Task DeleteTicketByPaymentTransactionIdAsync(string transactionId);
}
