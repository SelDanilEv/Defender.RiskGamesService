namespace Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;

public enum UserTicketStatus
{
    Requested,
    Paid,
    Won,
    PrizePaid,
    FailedToPayPrize,
    Lost,
}
