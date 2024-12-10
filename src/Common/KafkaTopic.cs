namespace Defender.RiskGamesService.Common;

public enum KafkaTopic
{
    ScheduledTasks,
    LotteryToProcess
}

public static class KafkaTopicExtensions
{
    private const string ServiceName = "RiskGamesService";

    private static class Topics
    {
        public const string ScheduleNewLotteryDraw = $"{ServiceName}_scheduled-tasks-topic";
        public const string LotteryToProcess = $"{ServiceName}_lottery-to-process";
    }

    private static readonly Dictionary<KafkaTopic, string> _topicToStringMap =
        new()
        {
            { KafkaTopic.ScheduledTasks, Topics.ScheduleNewLotteryDraw },
            { KafkaTopic.LotteryToProcess, Topics.LotteryToProcess },
        };


    public static string GetName(this KafkaTopic topic)
    {
        if (_topicToStringMap.TryGetValue(topic, out var name))
        {
            return name;
        }
        throw new ArgumentException($"Unknown topic: {topic}");
    }

    public static KafkaTopic ToTopic(this string topic)
    {
        var stringToTopicMap = _topicToStringMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        if (stringToTopicMap.TryGetValue(topic, out var result))
        {
            return result;
        }
        throw new ArgumentException($"Unknown topic: {topic}");
    }
}