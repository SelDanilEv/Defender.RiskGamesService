namespace Defender.RiskGamesService.Application.Models.Transaction;

public record StartTransactionResult
{
    public StartTransactionResult(
        TransactionModel transaction,
        Task createTransactionToTrackTask)
    {
        Transaction = transaction;
        CreateTransactionToTrackTask = createTransactionToTrackTask;
    }

    public TransactionModel Transaction { get; set; }
    public Task CreateTransactionToTrackTask { get; set; }

}
