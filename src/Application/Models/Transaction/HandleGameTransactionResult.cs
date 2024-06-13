namespace Defender.RiskGamesService.Application.Models.Transaction;

public record HandleGameTransactionResult
{
    private HandleGameTransactionResult(bool stopTracking)
    {
        StopTracking = stopTracking;
    }

    public static HandleGameTransactionResult StopTrackingResult
        => new HandleGameTransactionResult(true);

    public static HandleGameTransactionResult KeepTrackingResult
        => new HandleGameTransactionResult(false);

    public bool StopTracking { get; set; }

}
