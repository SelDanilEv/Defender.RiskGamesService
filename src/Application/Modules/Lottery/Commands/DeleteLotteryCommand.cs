using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using FluentValidation;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Commands;

public record DeleteLotteryCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
};

public sealed class DeleteLotteryCommandValidator : AbstractValidator<DeleteLotteryCommand>
{
    public DeleteLotteryCommandValidator()
    {
    }
}

public sealed class DeleteLotteryCommandHandler(
        ILotteryManagementService lotteryManagementService)
    : IRequestHandler<DeleteLotteryCommand, Guid>
{

    public async Task<Guid> Handle(
        DeleteLotteryCommand request,
        CancellationToken cancellationToken)
    {
        await lotteryManagementService.DeleteLotteryAsync(request.Id);

        return request.Id;
    }
}
