using Defender.Common.Clients.Wallet;
using Defender.Common.Mapping;
using Defender.RiskGamesService.Application.Models.Transaction;

namespace Defender.RiskGamesService.Infrastructure.Mappings;

public class MappingProfile : BaseMappingProfile
{
    public MappingProfile()
    {
        CreateMap<TransactionDto, TransactionModel>();
        CreateMap<AnonymousTransactionDto, AnonymousTransactionModel>();

        CreateMap<AnonymousTransactionDto, AnonymousTransactionModel>();

    }
}
