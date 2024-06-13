using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Application.Models.Lottery.Tickets;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;
using Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;

public interface IUserTicketManagementService
{
    Task<PagedResult<UserTicket>> GetUserTicketsAsync(PaginationRequest request, Guid userId);
    Task<List<UserTicket>> GetUserTicketsByDrawNumberAsync(long drawNumber);
    Task<IEnumerable<int>> PurchaseTicketsAsync(PurchaseLotteryTicketsRequest request);
    Task CheckWinningsAsync(LotteryDraw draw);
}
