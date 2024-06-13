namespace Defender.RiskGamesService.Domain.Entities.Lottery.TicketsSettings;

public class TicketsPrizeSetup
{
    public static TicketsPrizeSetup Create(
            List<TicketPrize>? prizes) =>
        new()
        {
            Prizes = prizes,
        };

    public List<TicketPrize>? Prizes { get; set; }
}
