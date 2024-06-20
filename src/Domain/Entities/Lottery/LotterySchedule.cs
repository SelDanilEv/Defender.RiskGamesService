using Defender.RiskGamesService.Domain.Entities.Lottery.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Defender.RiskGamesService.Domain.Entities.Lottery;

public class LotterySchedule
{
    public static LotterySchedule Create(
            LotteryScheduleType type,
            int? customHours,
            DateTime startDate,
            LotteryScheduleType? durationType = null,
            int? durationCustomDays = null) =>
        new()
        {
            Type = type,
            CustomHours = customHours ?? 1,
            NextStartDate = startDate,
            DurationType = durationType ?? type,
            DurationCustomHours = durationCustomDays ?? 1,
        };

    [BsonRepresentation(BsonType.String)]
    public LotteryScheduleType Type { get; set; }
    public int CustomHours { get; set; }

    public DateTime LastStartedDate { get; set; }
    public DateTime NextStartDate { get; set; }

    [BsonRepresentation(BsonType.String)]
    public LotteryScheduleType DurationType { get; set; }
    public int DurationCustomHours { get; set; }

    public LotterySchedule UpdateNextStartDate()
    {
        var now = DateTime.UtcNow;

        LastStartedDate = NextStartDate;

        while (NextStartDate < now)
        {
            NextStartDate = Type switch
            {
                LotteryScheduleType.Custom => NextStartDate.AddHours(CustomHours),
                LotteryScheduleType.Daily => NextStartDate.AddDays(1),
                LotteryScheduleType.Weekly => NextStartDate.AddDays(7),
                LotteryScheduleType.Monthly => NextStartDate.AddMonths(1),
                LotteryScheduleType.Yearly => NextStartDate.AddYears(1),
                _ => throw new NotImplementedException(),
            };

            if (NextStartDate < now)
                LastStartedDate = NextStartDate;
        }

        return this;
    }

}
