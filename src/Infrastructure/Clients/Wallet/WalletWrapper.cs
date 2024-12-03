using AutoMapper;
using Defender.Common.Clients.Wallet;
using Defender.Common.Helpers;
using Defender.Common.Interfaces;
using Defender.Common.Wrapper.Internal;
using Defender.RiskGamesService.Application.Common.Interfaces.Wrapper;
using Defender.RiskGamesService.Application.Models.Transaction;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Infrastructure.Clients.Wallet;

public class WalletWrapper(
    IAuthenticationHeaderAccessor authenticationHeaderAccessor,
    IWalletServiceClient serviceClient,
    IMapper mapper
    ) : BaseInternalSwaggerWrapper(
            serviceClient,
            authenticationHeaderAccessor
        ), IWalletWrapper
{
    public async Task<AnonymousTransactionModel> GetTransactionAsync(string transactionId)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var response = await serviceClient
                .StatusAsync(transactionId);

            return mapper.Map<AnonymousTransactionModel>(response);
        }, AuthorizationType.Service);
    }

    public async Task<TransactionModel> StartPaymentTransactionAsync(
        Guid userId,
        int amount,
        Currency currency,
        GameType gameType,
        string? comment = null)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var command = new StartPaymentTransactionCommand()
            {
                TargetUserId = userId,
                Amount = amount,
                Currency = MappingHelper.MapEnum
                    (currency, StartPaymentTransactionCommandCurrency.Unknown),
                TransactionPurpose = MappingHelper.MapEnum
                    (MapTransactionPurpose(gameType),
                    StartPaymentTransactionCommandTransactionPurpose.NoPurpose),
                Comment = comment,
            };

            var response = await serviceClient
                .PaymentAsync(command);

            return mapper.Map<TransactionModel>(response);
        }, AuthorizationType.User);
    }

    public async Task<TransactionModel> StartRechargeTransactionAsync(
        Guid userId,
        int amount,
        Currency currency,
        GameType gameType,
        string? comment = null)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var command = new StartRechargeTransactionCommand()
            {
                TargetUserId = userId,
                Amount = amount,
                Currency = MappingHelper.MapEnum
                    (currency, StartRechargeTransactionCommandCurrency.Unknown),
                TransactionPurpose = MappingHelper.MapEnum
                    (MapTransactionPurpose(gameType),
                    StartRechargeTransactionCommandTransactionPurpose.NoPurpose),
                Comment = comment,
            };

            var response = await serviceClient
                .RechargeAsync(command);

            return mapper.Map<TransactionModel>(response);
        }, AuthorizationType.Service);
    }


    private static StartPaymentTransactionCommandTransactionPurpose MapTransactionPurpose
        (GameType gameType) => gameType switch
        {
            GameType.Lottery => StartPaymentTransactionCommandTransactionPurpose.Lottery,
            _ => StartPaymentTransactionCommandTransactionPurpose.NoPurpose
        };

}
