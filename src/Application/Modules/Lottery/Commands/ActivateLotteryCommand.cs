using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using FluentValidation;
using Defender.Common.Extension;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Commands;

public record ActivateLotteryCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
};

public sealed class ActivateLotteryCommandValidator : AbstractValidator<ActivateLotteryCommand>
{
    public ActivateLotteryCommandValidator()
    {
    }
}

public sealed class ActivateLotteryCommandHandler(
        ILotteryManagementService lotteryManagementService)
    : IRequestHandler<ActivateLotteryCommand, Guid>
{

    public async Task<Guid> Handle(
        ActivateLotteryCommand request,
        CancellationToken cancellationToken)
    {
        await lotteryManagementService.ActivateLotteryAsync(request.Id);

        return request.Id;
    }
}
