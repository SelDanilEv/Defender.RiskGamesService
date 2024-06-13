using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Domain.Entities.Lottery.TicketsSettings;

public class TicketsSetup
{
    public static TicketsSetup Create(
            int ticketsAmount,
            int startTicketNumber,
            List<int>? allowedValues,
            bool isCustomValueAllowed,
            int minValue,
            int maxValue,
            List<Currency>? allowedCurrencies,
            List<TicketPrize>? prizes) =>
        new()
        {
            TicketsAmount = ticketsAmount,
            StartTicketNumber = startTicketNumber,
            PriceSetup = TicketsPriceSetup.Create(
                allowedValues,
                isCustomValueAllowed,
                minValue,
                maxValue,
                allowedCurrencies),
            PrizeSetup = TicketsPrizeSetup.Create(prizes),
        };

    public int TicketsAmount { get; set; }
    public int StartTicketNumber { get; set; }
    public TicketsPriceSetup? PriceSetup { get; set; }
    public TicketsPrizeSetup? PrizeSetup { get; set; }
}
