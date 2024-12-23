﻿using Defender.Common.DB.Model;
using Defender.Kafka.Default;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Common.Kafka;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;

namespace Defender.RiskGamesService.Application.Services.Lottery;

public class LotteryProcessingService(
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
                KafkaTopic.LotteryToProcess.GetName(),
                drawId,
                cancellationToken);
        }
    }

    public async Task HandleLotteryDraw(Guid drawId)
    {
        var tasks = new List<Task>();

        var draw = await lotteryDrawRepository.GetLotteryDrawAsync(drawId);

        if (!draw.IsProcessing || draw.IsProcessed)
        {
            return;
        }

        var finishDrawUpdateRequest = UpdateModelRequest<LotteryDraw>
            .Init(draw.Id)
            .Set(x => x.IsProcessing, false)
            .Set(x => x.IsProcessed, true);

        if (draw.Winnings.Any(x => x.Tickets.Count == 0))
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

            finishDrawUpdateRequest
                .SetIfNotNull(x => x.Winnings, winnings);

            draw.Winnings = winnings;
        }

        tasks.Add(userTicketManagementService.CheckWinningsAsync(draw));

        tasks.Add(lotteryDrawRepository.UpdateLotteryDrawAsync(finishDrawUpdateRequest));

        await Task.WhenAll(tasks);
    }
}
