using Defender.Common.DB.SharedStorage.Enums;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Application.Models.Transaction;

public record TransactionRequest
{
    private Guid? _userId = null;

    public TransactionRequest(
        string drawId,
        int amount,
        Currency currency,
        TransactionType transactionType,
        GameType gameType,
        string? comment = null)
    {
        DrawId = drawId;
        Amount = amount;
        TransactionType = transactionType;
        Currency = currency;
        GameType = gameType;
        Comment = comment ?? string.Empty;
    }

    public string DrawId { get; set; }
    public int Amount { get; set; }
    public TransactionType TransactionType { get; set; } = TransactionType.Unknown;
    public Currency Currency { get; set; }
    public GameType GameType { get; set; } = GameType.Undefined;
    public string Comment { get; set; } = string.Empty;

    public Guid UserId => _userId ?? Guid.Empty;
    public TransactionRequest SetUserId(Guid userId)
    {
        _userId = userId;

        return this;
    }

    public bool AsUser => !_userId.HasValue;
}
