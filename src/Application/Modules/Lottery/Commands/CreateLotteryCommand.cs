using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Application.Models.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery;
using FluentValidation;
using Defender.Common.Extension;
using MediatR;

namespace Defender.RiskGamesService.Application.Modules.Lottery.Commands;

public record CreateLotteryCommand : CreateLotteryRequest, IRequest<LotteryModel>
{
};

public sealed class CreateLotteryCommandValidator : AbstractValidator<CreateLotteryCommand>
{
    public CreateLotteryCommandValidator()
    {
    }
}

public sealed class CreateLotteryCommandHandler(
        ILotteryManagementService lotteryManagementService)
    : IRequestHandler<CreateLotteryCommand, LotteryModel>
{

    public async Task<LotteryModel> Handle(
        CreateLotteryCommand request,
        CancellationToken cancellationToken)
    {
        return await lotteryManagementService.CreateLotteryAsync(request);
    }
}
