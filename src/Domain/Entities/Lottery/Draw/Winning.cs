namespace Defender.RiskGamesService.Domain.Entities.Lottery.Draw;

public class Winning
{
    public int Coefficient { get; set; }
    public List<int> Tickets { get; set; } = [];
}

