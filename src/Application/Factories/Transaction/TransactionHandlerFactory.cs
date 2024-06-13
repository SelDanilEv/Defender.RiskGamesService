using Defender.Common.DB.SharedStorage.Enums;
using Defender.Common.Errors;
using Defender.Common.Exceptions;
using Defender.RiskGamesService.Application.Common.Interfaces.Handlers;
using Defender.RiskGamesService.Application.Handlers.Transaction;
using Defender.RiskGamesService.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Defender.RiskGamesService.Application.Factories.Transaction;

public class TransactionHandlerFactory(IServiceProvider serviceProvider)
{
    public IGameTransactionHandler CreateGameTransactionHandler(GameType gameType)
    {
        return gameType switch
        {
            GameType.Lottery => serviceProvider.GetRequiredService<LotteryTransactionHandler>(),
            _ => throw new ServiceException(ErrorCode.BR_RGS_UnsupportedGameType)
        };
    }

    public IStartTransactionHandler StartTransactionHandler(TransactionType transactionType)
    {
        return transactionType switch
        {
            TransactionType.Recharge => serviceProvider.GetRequiredService<StartRechargeTransactionHandler>(),
            TransactionType.Payment => serviceProvider.GetRequiredService<StartPaymentTransactionHandler>(),
            _ => throw new ServiceException(ErrorCode.BR_RGS_UnsupportedTransactionType)
        };
    }
}

