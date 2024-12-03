using Defender.Common.Errors;
using Defender.Common.Extension;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Application.Models.Lottery.Tickets;
using FluentValidation;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Commands.Tickets;

public record PurchaseLotteryTicketCommand
    : PurchaseLotteryTicketsRequest, IRequest<IEnumerable<int>>
{
};

public sealed class PurchaseLotteryTicketCommandValidator
        : AbstractValidator<PurchaseLotteryTicketCommand>
{
    public PurchaseLotteryTicketCommandValidator()
    {
        RuleFor(x => x.DrawNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorCode.VL_RGS_InvalidDrawNumber);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage(ErrorCode.VL_RGS_InvalidAmount);

        RuleForEach(x => x.TicketNumbers)
            .GreaterThan(0)
            .WithMessage(ErrorCode.VL_RGS_InvalidTicketNumber);
    }
}

public sealed class PurchaseLotteryTicketCommandHandler(
        IUserTicketManagementService userTicketManagementService)
    : IRequestHandler<PurchaseLotteryTicketCommand, IEnumerable<int>>
{

    public async Task<IEnumerable<int>> Handle(
        PurchaseLotteryTicketCommand request,
        CancellationToken cancellationToken)
    {
        await userTicketManagementService.PurchaseTicketsAsync(request);

        return request.TicketNumbers;
    }
}
