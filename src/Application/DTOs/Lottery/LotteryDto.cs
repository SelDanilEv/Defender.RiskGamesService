using Defender.RiskGamesService.Domain.Entities.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery.TicketsSettings;

namespace Defender.RiskGamesService.Application.DTOs.Lottery;

public record LotteryDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Dictionary<string, string>? PublicNames { get; set; }
    public LotterySchedule? Schedule { get; set; }
    public TicketsSetup? TicketsSetup { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsActive { get; set; }
    public int IncomePercentage =>
        TicketsSetup?.TicketsAmount * 100 -
        TicketsSetup?.PrizeSetup?.Prizes?.Sum(
            prize => prize.TicketsAmount * prize.Coefficient)
        ?? -1;

}
