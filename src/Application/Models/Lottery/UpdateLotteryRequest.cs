namespace Defender.RiskGamesService.Application.Models.Lottery;

public record UpdateLotteryRequest : BaseLotteryRequest
{
    public Guid Id { get; set; }
    public bool? IsActive { get; set; }
}