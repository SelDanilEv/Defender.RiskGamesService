using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Application.Models.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery;
using FluentValidation;
using Defender.Common.Extension;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Commands;

public record UpdateLotteryCommand : UpdateLotteryRequest, IRequest<LotteryModel>
{
};

public sealed class UpdateLotteryCommandValidator : AbstractValidator<UpdateLotteryCommand>
{
    public UpdateLotteryCommandValidator()
    {
    }
}

public sealed class UpdateLotteryCommandHandler(
        ILotteryManagementService lotteryManagementService)
    : IRequestHandler<UpdateLotteryCommand, LotteryModel>
{

    public async Task<LotteryModel> Handle(
        UpdateLotteryCommand request,
        CancellationToken cancellationToken)
    {
        return await lotteryManagementService.UpdateLotteryAsync(request);
    }
}
