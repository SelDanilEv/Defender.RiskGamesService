using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.Common.Errors;
using Defender.Common.Exceptions;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Application.Models.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;
using Defender.RiskGamesService.Domain.Entities.Lottery.TicketsSettings;

namespace Defender.RiskGamesService.Application.Services.Lottery;

public class LotteryManagementService(
        ILotteryRepository lotteryRepository,
        ILotteryDrawRepository lotteryDrawRepository)
    : ILotteryManagementService
{
    #region Lotteries

    public async Task<LotteryModel> GetLotteryAsync(Guid lotteryId)
    {
        return await lotteryRepository.GetLotteryModelByIdAsync(lotteryId);
    }

    public async Task<PagedResult<LotteryModel>> GetLotteriesAsync(
        PaginationRequest request, string? name = "")
    {
        return await lotteryRepository.GetLotteriesAsync(request, name);
    }

    public async Task<LotteryModel> CreateLotteryAsync(CreateLotteryRequest createRequest)
    {
        if (createRequest is null || createRequest.FirstTicketNumber is null
            || createRequest.TicketsAmount is null || createRequest.IsCustomValueAllowed is null
            || createRequest.MinBet is null || createRequest.MaxBet is null)
        {
            throw new ServiceException(ErrorCode.VL_InvalidRequest);
        }

        var newLottery = new LotteryModel
        {
            Name = createRequest.Name,
            PublicNames = createRequest.PublicNames,
            Schedule = LotterySchedule.Create(
                createRequest.ScheduleType,
                createRequest.ScheduleCustomHours,
                createRequest.StartDate,
                createRequest.DurationType,
                createRequest.DurationCustomHours),
            TicketsSetup = TicketsSetup.Create(
                createRequest.TicketsAmount.Value,
                createRequest.FirstTicketNumber.Value,
                createRequest.AllowedValues,
                createRequest.IsCustomValueAllowed.Value,
                createRequest.MinBet.Value,
                createRequest.MaxBet.Value,
                createRequest.AllowedCurrencies,
                createRequest.Prizes),
        };

        return await lotteryRepository.CreateNewLotteryAsync(newLottery);
    }

    public async Task<LotteryModel> UpdateLotteryAsync(UpdateLotteryRequest updateRequest)
    {
        if (updateRequest == null)
        {
            throw new ServiceException(ErrorCode.VL_InvalidRequest);
        }

        var request = UpdateModelRequest<LotteryModel>.Init(updateRequest.Id)
            .SetIfNotNull(x => x.Name, updateRequest.Name)
            .SetIfNotNull(x => x.PublicNames, updateRequest.PublicNames)
            .SetIfNotNull(x => x.Schedule!.Type, updateRequest.ScheduleType)
            .SetIfNotNull(x => x.Schedule!.CustomHours, updateRequest.ScheduleCustomHours)
            .SetIfNotNull(x => x.Schedule!.NextStartDate, updateRequest.StartDate)
            .SetIfNotNull(x => x.Schedule!.DurationType, updateRequest.DurationType)
            .SetIfNotNull(x => x.Schedule!.DurationCustomHours, updateRequest.DurationCustomHours)
            .SetIfNotNull(x => x.TicketsSetup!.StartTicketNumber, updateRequest.FirstTicketNumber)
            .SetIfNotNull(x => x.TicketsSetup!.TicketsAmount, updateRequest.TicketsAmount)
            .SetIfNotNull(x => x.TicketsSetup!.PriceSetup!.AllowedValues, updateRequest.AllowedValues)
            .SetIfNotNull(x => x.TicketsSetup!.PriceSetup!.IsCustomValueAllowed, updateRequest.IsCustomValueAllowed)
            .SetIfNotNull(x => x.TicketsSetup!.PriceSetup!.MinValue, updateRequest.MinBet)
            .SetIfNotNull(x => x.TicketsSetup!.PriceSetup!.MaxValue, updateRequest.MaxBet)
            .SetIfNotNull(x => x.TicketsSetup!.PriceSetup!.AllowedCurrencies, updateRequest.AllowedCurrencies)
            .SetIfNotNull(x => x.TicketsSetup!.PrizeSetup!.Prizes, updateRequest.Prizes)
            .SetIfNotNull(x => x.IsActive, updateRequest.IsActive);

        if (updateRequest.TicketsAmount.HasValue || updateRequest.Prizes != null)
        {
            int newIncomePercentage = 0;

            if (updateRequest.TicketsAmount.HasValue && updateRequest.Prizes != null)
            {
                newIncomePercentage = updateRequest.TicketsAmount.Value * 100
                    - updateRequest.Prizes.Sum(
                        prize => prize.TicketsAmount * prize.Coefficient);
            }
            else
            {
                var model = await lotteryRepository
                    .GetLotteryModelByIdAsync(updateRequest.Id);

                newIncomePercentage =
                    updateRequest.TicketsAmount ?? model.TicketsSetup?.TicketsAmount
                    * 100
                    - (updateRequest.Prizes ?? model.TicketsSetup?.PrizeSetup?.Prizes)?
                        .Sum(prize => prize.TicketsAmount * prize.Coefficient)
                ?? -1;
            }

            request.SetIfNotNull(x => x.IncomePercentage, newIncomePercentage);
        }

        return await lotteryRepository.UpdateLotteryAsync(request);
    }

    public async Task ActivateLotteryAsync(Guid lotteryId)
    {
        var request = UpdateModelRequest<LotteryModel>.Init(lotteryId)
            .SetIfNotNull(x => x.IsActive, true);

        await lotteryRepository.UpdateLotteryAsync(request);
    }

    public async Task DeactivateLotteryAsync(Guid lotteryId)
    {
        var request = UpdateModelRequest<LotteryModel>.Init(lotteryId)
            .SetIfNotNull(x => x.IsActive, false);

        await lotteryRepository.UpdateLotteryAsync(request);
    }

    public async Task DeleteLotteryAsync(Guid lotteryId)
    {
        await lotteryRepository.DeleteLotteryAsync(lotteryId);
    }

    #endregion

    #region Lottery Draw

    public async Task ScheduleDraws()
    {
        var lotteries = await lotteryRepository.GetAllLotteriesToScheduleAsync();

        foreach (var lottery in lotteries)
        {
            var draw = LotteryDraw.Create(lottery);
            
            var filter = FindModelRequest<LotteryModel>
                .Init(x => x.Id, lottery.Id)
                .And(x => x.IsActive, true)
                .And(x => x.IncomePercentage, 0, FilterType.Gt)
                .And(x => x.Schedule!.NextStartDate, DateTime.UtcNow, FilterType.Lte);
                
            var updateRequest = UpdateModelRequest<LotteryModel>
                .Init(lottery.Id)
                .SetIfNotNull(x => x.Schedule, lottery.Schedule!.UpdateNextStartDate());

            var updatedLottery = await lotteryRepository.UpdateLotteryAsync(updateRequest, filter);

            if (updatedLottery is not null)
            {
                await lotteryDrawRepository.CreateLotteryDrawAsync(draw);
            }
        }
    }

    public Task<LotteryDraw> GetLotteryDrawByNumberAsync(long drawNumber)
    {
        return lotteryDrawRepository.GetLotteryDrawAsync(drawNumber);
    }

    public Task<PagedResult<LotteryDraw>> GetActiveDrawAsync(PaginationRequest request)
    {
        return lotteryDrawRepository.GetActiveLotteryDrawsAsync(request);
    }

    #endregion Lottery Draw
}
