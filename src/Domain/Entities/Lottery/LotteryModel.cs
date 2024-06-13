using Defender.Common.Entities;
using Defender.RiskGamesService.Domain.Entities.Lottery.TicketsSettings;
using MongoDB.Bson.Serialization.Attributes;

namespace Defender.RiskGamesService.Domain.Entities.Lottery;

public class LotteryModel : IBaseModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Dictionary<string, string>? PublicNames { get; set; }
    public LotterySchedule? Schedule { get; set; }
    public TicketsSetup? TicketsSetup { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsActive { get; set; }

    [BsonElement(nameof(IncomePercentage))]
    public int IncomePercentage =>
        (this.TicketsSetup?.TicketsAmount * 100  - 
        this.TicketsSetup?.PrizeSetup?.Prizes?.Sum(
            prize => prize.TicketsAmount * prize.Coefficient)) 
        ?? -1;
}
