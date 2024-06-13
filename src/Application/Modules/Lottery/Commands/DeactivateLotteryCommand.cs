using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using FluentValidation;
using Defender.Common.Extension;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Commands;

public record DeactivateLotteryCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
};

public sealed class DeactivateLotteryCommandValidator : AbstractValidator<DeactivateLotteryCommand>
{
    public DeactivateLotteryCommandValidator()
    {
    }
}

public sealed class DeactivateLotteryCommandHandler(
        ILotteryManagementService lotteryManagementService)
    : IRequestHandler<DeactivateLotteryCommand, Guid>
{

    public async Task<Guid> Handle(
        DeactivateLotteryCommand request,
        CancellationToken cancellationToken)
    {
        await lotteryManagementService.DeactivateLotteryAsync(request.Id);

        return request.Id;
    }
}
