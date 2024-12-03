using Defender.Common.Entities;
using Defender.RiskGamesService.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;

public class UserTicket : IBaseModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public long DrawNumber { get; set; }
    public int TicketNumber { get; set; }
    public int Amount { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Currency Currency { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
    public Guid UserId { get; set; }
    public string? PaymentTransactionId { get; set; }
    public string? PrizeTransactionId { get; set; }
    public int PrizePaidAmount { get; set; }
    [BsonRepresentation(BsonType.String)]
    public UserTicketStatus Status { get; set; }

    public DateTime PurchaseDate { get; set; }

}
