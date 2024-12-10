using Defender.Common.DB.Model;
using Defender.Common.Extension;
using Defender.Common.Kafka.Default;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;
using Defender.RiskGamesService.Common;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;
using Defender.RiskGamesService.Domain.Enums;
using Microsoft.Extensions.Hosting;

namespace Defender.RiskGamesService.Application.Services.Lottery;

public class LotteryProcessingService(
        IHostEnvironment hostEnvironment,
        IDefaultKafkaProducer<Guid> kafkaProducer,
        ILotteryDrawRepository lotteryDrawRepository,
        IUserTicketManagementService userTicketManagementService)
    : ILotteryProcessingService
{
    public async Task QueueLotteriesForProcessing(CancellationToken cancellationToken = default)
    {
        var drawIds = await lotteryDrawRepository
            .GetLotteryDrawsToProcessAsync(cancellationToken);

        foreach (var drawId in drawIds)
        {
            await kafkaProducer.ProduceAsync(
                KafkaTopic.LotteryToProcess.GetName(hostEnvironment.GetAppEnvironment()),
                drawId,
                cancellationToken);
        }
    }

    public async Task HandleLotteryDraw(Guid drawId)
    {
        var tasks = new List<Task>();
        
        var draw = await lotteryDrawRepository.GetLotteryDrawAsync(drawId);
        
        if(!draw.IsProcessing || draw.IsProcessed)
        {
            return;
        }

        if (draw.Winnings.Count == 0)
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
                .SetIfNotNull(x => x.Winnings, winnings)
                .Set(x => x.IsProcessing, false)
                .Set(x => x.IsProcessed, true);

            draw.Winnings = winnings;
            
            tasks.Add(lotteryDrawRepository.UpdateLotteryDrawAsync(updateRequest));
        }
        
        tasks.Add(userTicketManagementService.CheckWinningsAsync(draw));

        await Task.WhenAll(tasks);
    }
}
