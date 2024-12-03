using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery;
using FluentValidation;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Queries;

public record GetLotteriesQuery : PaginationRequest, IRequest<PagedResult<LotteryModel>>
{
    public string? Name { get; init; }
};

public sealed class GetLotteriesQueryValidator : AbstractValidator<GetLotteriesQuery>
{
    public GetLotteriesQueryValidator()
    {
    }
}

public sealed class GetLotteriesQueryHandler(
        ILotteryManagementService lotteryManagementService)
    : IRequestHandler<GetLotteriesQuery, PagedResult<LotteryModel>>
{

    public async Task<PagedResult<LotteryModel>> Handle(
        GetLotteriesQuery request,
        CancellationToken cancellationToken)
    {
        return await lotteryManagementService.GetLotteriesAsync(request, request.Name);
    }
}
