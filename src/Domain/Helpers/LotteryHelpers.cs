namespace Defender.RiskGamesService.Domain.Helpers;

public static class LotteryHelpers
{
    public static string PurchaseLotteryTicketsTransactionComment(
        long drawNumber, IEnumerable<int> tickets) =>
            $"{drawNumber}: {string.Join(", ", tickets)}";
    public static string LotteryPrizeTransactionComment(
        long drawNumber, int ticket, int bet, int coefficient) =>
            $"{drawNumber} ({ticket}): {AsCurrency(bet)} * {coefficient}% = {AsCurrency(
                    CalculateLotteryPrizeAmount(bet, coefficient))}";

    public static int CalculateLotteryPrizeAmount(int amount, int coefficient)
    {
        return amount * coefficient / 100;
    }

    public static double AsCurrency(int amount) => amount / 100.0;
}
