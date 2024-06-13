using AutoMapper;
using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;
using FluentValidation;
using Defender.Common.Extension;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Queries;

public record GetLotteryDrawsQuery
    : PaginationRequest, IRequest<PagedResult<LotteryDraw>>
{
};

public sealed class GetLotteryDrawsQueryValidator : AbstractValidator<GetLotteryDrawsQuery>
{
    public GetLotteryDrawsQueryValidator()
    {
    }
}

public sealed class GetLotteryDrawsQueryHandler(
        ILotteryManagementService lotteryManagementService)
    : IRequestHandler<GetLotteryDrawsQuery, PagedResult<LotteryDraw>>
{

    public Task<PagedResult<LotteryDraw>> Handle(
        GetLotteryDrawsQuery request,
        CancellationToken cancellationToken)
    {
        return lotteryManagementService.GetActiveDrawAsync(request);
    }
}
