using Defender.Common.Mapping;
using Defender.RiskGamesService.Application.DTOs.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;
using Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;

namespace Defender.RiskGamesService.Application.Mappings;

public class MappingProfile : BaseMappingProfile
{
    public MappingProfile()
    {
        CreateMap<LotteryDraw, LotteryDrawDto>()
            .ForMember(dest => dest.Coefficients,
               opt => opt.MapFrom(
                   src => src.Winnings.Select(w => w.Coefficient).ToList()));
        CreateMap<LotteryModel, LotteryDto>();
        CreateMap<UserTicket, UserTicketDto>();
    }
}
