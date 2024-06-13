using Defender.Common.Interfaces;
using Defender.RiskGamesService.Application.Common.Interfaces.Handlers;
using Defender.RiskGamesService.Application.Common.Interfaces.Wrapper;
using Defender.RiskGamesService.Application.Models.Transaction;

namespace Defender.RiskGamesService.Application.Handlers.Transaction;


public class StartRechargeTransactionHandler(
    ICurrentAccountAccessor currentAccountAccessor,
    IWalletWrapper walletWrapper)
    : IStartTransactionHandler
{

    public Task<TransactionModel> HandleStartTransactionAsync(TransactionRequest request)
    {
        if (request.AsUser)
            request.SetUserId(currentAccountAccessor.GetAccountId());

        return walletWrapper.StartRechargeTransactionAsync(
                    request.UserId,
                    request.Amount,
                    request.Currency,
                    request.GameType,
                    request.Comment);
    }
}

