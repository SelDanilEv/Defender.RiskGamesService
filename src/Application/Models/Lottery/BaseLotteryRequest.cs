using Defender.RiskGamesService.Domain.Entities.Lottery.Enums;
using Defender.RiskGamesService.Domain.Entities.Lottery.TicketsSettings;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Application.Models.Lottery;

public record BaseLotteryRequest
{
    public string? Name { get; set; }
    public Dictionary<string, string>? PublicNames { get; set; }
    public LotteryScheduleType ScheduleType { get; set; }
    public int? ScheduleCustomHours { get; set; }
    public LotteryScheduleType? DurationType { get; set; }
    public int? DurationCustomHours { get; set; }
    public DateTime StartDate { get; set; }

    public int? FirstTicketNumber { get; set; }
    public int? TicketsAmount { get; set; }
    public List<int>? AllowedValues { get; set; }
    public bool? IsCustomValueAllowed { get; set; }
    public int? MinBet { get; set; }
    public int? MaxBet { get; set; }
    public List<Currency>? AllowedCurrencies { get; set; }
    public List<TicketPrize>? Prizes { get; set; }
}
