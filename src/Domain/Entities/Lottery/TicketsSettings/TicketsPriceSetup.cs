using Defender.RiskGamesService.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Defender.RiskGamesService.Domain.Entities.Lottery.TicketsSettings;

public class TicketsPriceSetup
{
    public static TicketsPriceSetup Create(
            List<int>? allowedValues,
            bool isCustomValueAllowed,
            int minValue,
            int maxValue,
            List<Currency>? allowedCurrencies) =>
        new TicketsPriceSetup
        {
            AllowedValues = allowedValues,
            IsCustomValueAllowed = isCustomValueAllowed,
            MinValue = minValue,
            MaxValue = maxValue,
            AllowedCurrencies = allowedCurrencies ?? new List<Currency>
            {
                Currency.USD,
            },
        };

    public List<int>? AllowedValues { get; set; }
    public bool IsCustomValueAllowed { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    [BsonRepresentation(BsonType.String)]
    public List<Currency>? AllowedCurrencies { get; set; }
}
