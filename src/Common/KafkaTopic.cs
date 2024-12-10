using Defender.Common.Enums;

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

    private static readonly Dictionary<KafkaTopic, string> TopicToStringMap =
        new()
        {
            { KafkaTopic.ScheduledTasks, Topics.ScheduleNewLotteryDraw },
            { KafkaTopic.LotteryToProcess, Topics.LotteryToProcess },
        };


    public static string GetName(this KafkaTopic topic, AppEnvironment env)
    {
        if (TopicToStringMap.TryGetValue(topic, out var name))
        {
            return $"{env}_{name}";
        }
        throw new ArgumentException($"Unknown topic: {topic}");
    }
}