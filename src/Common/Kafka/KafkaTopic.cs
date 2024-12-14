namespace Defender.RiskGamesService.Common.Kafka;

public enum KafkaTopic
{
    ScheduledTasks,
    LotteryToProcess
}

public static class KafkaTopicExtensions
{
    private const string ServiceName = "RiskGamesService";

    private static readonly Dictionary<KafkaTopic, string> TopicToStringMap =
        new()
        {
            { KafkaTopic.ScheduledTasks, $"{ServiceName}_scheduled-tasks-topic" },
            { KafkaTopic.LotteryToProcess, $"{ServiceName}_lottery-to-process" },
        };


    public static string GetName(this KafkaTopic topic)
    {
        if (TopicToStringMap.TryGetValue(topic, out var name))
        {
            return name;
        }

        throw new ArgumentException($"Unknown topic: {topic}");
    }
}