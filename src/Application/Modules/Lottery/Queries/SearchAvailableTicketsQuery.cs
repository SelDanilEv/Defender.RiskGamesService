using Defender.Common.Errors;
using Defender.Common.Exceptions;
using Defender.Common.Extension;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using FluentValidation;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Queries;

public record SearchAvailableTicketsQuery : IRequest<List<int>>
{
    public int TargetTicket { get; init; } = 0;
    public int AmountOfTickets { get; init; }
    public long DrawNumber { get; init; }
    public bool IsRandom() => TargetTicket == 0;
};

public sealed class SearchAvailableTicketsQueryValidator
    : AbstractValidator<SearchAvailableTicketsQuery>
{
    public SearchAvailableTicketsQueryValidator()
    {
        RuleFor(x => x.AmountOfTickets)
            .GreaterThan(0)
            .WithMessage(ErrorCode.VL_RGS_AmountOfTicketsMustBePositive);

        RuleFor(x => x.DrawNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorCode.VL_RGS_InvalidDrawNumber);
    }
}

public sealed class SearchAvailableTicketsQueryHandler(
        IUserTicketManagementService userTicketManagementService,
        ILotteryManagementService lotteryManagementService)
    : IRequestHandler<SearchAvailableTicketsQuery, List<int>>
{
    public async Task<List<int>> Handle(
        SearchAvailableTicketsQuery request,
        CancellationToken cancellationToken)
    {
        var findTakenTicketsTask = userTicketManagementService
            .GetUserTicketsByDrawNumberAsync(request.DrawNumber);

        var findLotteryDrawInfoTask = lotteryManagementService
            .GetLotteryDrawByNumberAsync(request.DrawNumber);

        var takenTickets = await findTakenTicketsTask;
        var draw = await findLotteryDrawInfoTask;

        if (draw == null || !draw.IsActive)
            throw new ServiceException(ErrorCode.BR_RGS_LotteryDrawIsNotActive);

        var numbers = Enumerable.Range(
            draw.MinTicketNumber,
            draw.TicketsAmount).ToList();

        var takenTicketNumbers = new HashSet<int>(takenTickets.Select(x => x.TicketNumber));
        var freeNumbers = numbers.Where(n => !takenTicketNumbers.Contains(n)).ToArray();

        if (request.IsRandom())
        {
            Random.Shared.Shuffle(freeNumbers);
            return
            [
                .. freeNumbers
                    .Take(request.AmountOfTickets)
                    .OrderBy(x => x)
            ];
        }
        else
        {
            return
            [
                .. freeNumbers
                    .OrderBy(x => Math.Abs(x - request.TargetTicket))
                    .Take(request.AmountOfTickets)
                    .OrderBy(x => x)
            ];
        }
    }
}
