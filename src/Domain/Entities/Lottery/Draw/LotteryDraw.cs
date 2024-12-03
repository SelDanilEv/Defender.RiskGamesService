using Defender.Common.Entities;
using Defender.RiskGamesService.Domain.Entities.Lottery.Enums;
using Defender.RiskGamesService.Domain.Entities.Lottery.TicketsSettings;
using Defender.RiskGamesService.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Defender.RiskGamesService.Domain.Entities.Lottery.Draw;

public class LotteryDraw : IBaseModel
{
    public static LotteryDraw Create(LotteryModel model)
    {
        var (startDate, endDate) = GetDrawStartAndEndDate(model.Schedule!);

        return new LotteryDraw
        {
            LotteryId = model.Id,
            PublicNames = model.PublicNames ?? [],
            StartDate = startDate,
            EndDate = endDate,
            PrizeSetup = model.TicketsSetup?.PrizeSetup ?? new TicketsPrizeSetup(),
            AllowedCurrencies = model.TicketsSetup?.PriceSetup?.AllowedCurrencies
                ?? [Currency.USD],
            AllowedBets = model.TicketsSetup?.PriceSetup?.AllowedValues,
            MinBetValue = model.TicketsSetup?.PriceSetup?.MinValue ?? 0,
            MaxBetValue = model.TicketsSetup?.PriceSetup?.MaxValue ?? 0,
            IsCustomBetAllowed = model.TicketsSetup?.PriceSetup?.IsCustomValueAllowed ?? false,
            MinTicketNumber = model.TicketsSetup!.StartTicketNumber,
            MaxTicketNumber = model.TicketsSetup!.StartTicketNumber
                + model.TicketsSetup!.TicketsAmount - 1,
            Winnings = model!.TicketsSetup!.PrizeSetup!.Prizes!
                .Select(x => new Winning { Coefficient = x.Coefficient, Tickets = [] })
                .ToList(),
        };
    }

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public Dictionary<string, string> PublicNames { get; set; } = [];
    public long DrawNumber { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
    public Guid LotteryId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public TicketsPrizeSetup PrizeSetup { get; set; } = new TicketsPrizeSetup();
    [BsonRepresentation(BsonType.String)]
    public List<Currency> AllowedCurrencies { get; set; } = [Currency.USD];
    public List<int> AllowedBets { get; set; } = [];
    public int MinBetValue { get; set; }
    public int MaxBetValue { get; set; }
    public bool IsCustomBetAllowed { get; set; }

    public int MinTicketNumber { get; set; }
    public int MaxTicketNumber { get; set; }

    public List<Winning> Winnings { get; set; } = [];

    public bool IsProcessing { get; set; } = false;
    public bool IsProcessed { get; set; } = false;
    public bool IsActive => StartDate < DateTime.UtcNow && DateTime.UtcNow < EndDate;
    public int TicketsAmount => MaxTicketNumber - MinTicketNumber + 1;


    public static (DateTime, DateTime) GetDrawStartAndEndDate(LotterySchedule schedule)
    {
        var endDate = schedule.NextStartDate;
        var startDate = endDate;
        var now = DateTime.UtcNow;

        while (endDate < now)
        {
            endDate = schedule.DurationType switch
            {
                LotteryScheduleType.Custom => endDate.AddHours(schedule.DurationCustomHours),
                LotteryScheduleType.Daily => endDate.AddDays(1),
                LotteryScheduleType.Weekly => endDate.AddDays(7),
                LotteryScheduleType.Monthly => endDate.AddMonths(1),
                LotteryScheduleType.Yearly => endDate.AddYears(1),
                _ => throw new NotImplementedException(),
            };

            if (endDate < now)
            {
                startDate = endDate;
            }
        }

        return (startDate, endDate);
    }
}
