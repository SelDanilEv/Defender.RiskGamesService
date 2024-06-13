using Defender.Common.DB.Pagination;
using Defender.Common.Interfaces;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;
using FluentValidation;
using Defender.Common.Extension;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Queries;

public record GetUserTicketsQuery : PaginationRequest, IRequest<PagedResult<UserTicket>>
{
    public Guid UserId { get; init; } = Guid.Empty;
};

public sealed class GetUserTicketsQueryValidator : AbstractValidator<GetUserTicketsQuery>
{
    public GetUserTicketsQueryValidator()
    {
    }
}

public sealed class GetUserTicketsQueryHandler(
        IAuthorizationCheckingService authorizationCheckingService,
        ICurrentAccountAccessor currentAccountAccessor,
        IUserTicketManagementService userTicketManagementService)
    : IRequestHandler<GetUserTicketsQuery, PagedResult<UserTicket>>
{

    public async Task<PagedResult<UserTicket>> Handle(
        GetUserTicketsQuery request,
        CancellationToken cancellationToken)
    {
        return request.UserId == Guid.Empty
            ? await userTicketManagementService.GetUserTicketsAsync(
                request, currentAccountAccessor.GetAccountId())
            : await authorizationCheckingService.ExecuteWithAuthCheckAsync(
                request.UserId,
                async () => await userTicketManagementService.GetUserTicketsAsync(
                    request, request.UserId)
                );
    }
}
