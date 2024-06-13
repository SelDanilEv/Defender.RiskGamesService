using Defender.Common.DB.SharedStorage.Enums;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Application.Mappings;

public class TransactionMapper
{
    public static GameType MapGameType(TransactionPurpose transactionPurpose)
    {
        return transactionPurpose switch
        {
            TransactionPurpose.Lottery => GameType.Lottery,
            _ => GameType.Undefined,
        };
    }
}
