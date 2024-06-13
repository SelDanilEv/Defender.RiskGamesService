using Defender.Common.DB.Model;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;
using Defender.RiskGamesService.Domain.Entities.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Application.Services.Lottery;

public class LotteryProcessingService(
    ILotteryRepository lotteryRepository,
    ILotteryDrawRepository lotteryDrawRepository,
    IUserTicketManagementService userTicketManagementService,
    ITransactionManagementService transactionManagementService)
    : ILotteryProcessingService
{
    public async Task ScanAndProcessLotteries()
    {
        await ScheduleDraws();

        await lotteryDrawRepository.ProcessLotteryDrawsAsync(HangleLotteryDraw);
    }

    private async Task ScheduleDraws()
    {
        var lotteries = await lotteryRepository.GetAllLotteriesToScheduleAsync();

        var tasks = new List<Task>(lotteries.Count);
        foreach (var lottery in lotteries)
        {
            var draw = LotteryDraw.Create(lottery);
            var updateRequest = UpdateModelRequest<LotteryModel>
                .Init(lottery.Id)
                .SetIfNotNull(x => x.Schedule, lottery.Schedule!.UpdateNextStartDate());

            tasks.Add(lotteryDrawRepository.CreateLotteryDrawAsync(draw));
            _ = lotteryRepository.UpdateLotteryAsync(updateRequest);
        }

        await Task.WhenAll(tasks);
    }

    private async Task HangleLotteryDraw(LotteryDraw draw)
    {
        var numbers = Enumerable.Range(
            draw.MinTicketNumber,
            draw.TicketsAmount).ToArray();
        Random.Shared.Shuffle(numbers);

        var winnings = new List<Winning>();

        int takenSoFar = 0;
        foreach (var prize in draw.PrizeSetup!.Prizes!)
        {
            var winningTickets = numbers
                .Skip(takenSoFar)
                .Take(prize.TicketsAmount)
                .ToList();

            winningTickets.Sort();

            winnings.Add(new Winning
            {
                Coefficient = prize.Coefficient,
                Tickets = winningTickets
            });

            takenSoFar += prize.TicketsAmount;
        }

        var updateRequest = UpdateModelRequest<LotteryDraw>
            .Init(draw.Id)
            .SetIfNotNull(x => x.Winnings, winnings);

        List<Task> tasks = [];

        tasks.Add(lotteryDrawRepository
            .UpdateLotteryDrawAsync(updateRequest));

        await transactionManagementService
            .CheckUnhandledTicketsForDrawAsync(
                draw.DrawNumber.ToString(), GameType.Lottery);

        draw.Winnings = winnings;
        tasks.Add(userTicketManagementService
            .CheckWinningsAsync(draw));

        await Task.WhenAll(tasks);
    }

}
