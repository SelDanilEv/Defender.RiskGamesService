﻿namespace Defender.RiskGamesService.Common;

public enum KafkaEvent
{
    Unknown,
    ScheduleNewLotteryDraws,
    StartLotteriesProcessing
}

public static class KafkaEventExtensions
{
    public static string GetName(this KafkaEvent @event)
    {
        return @event.ToString();
    }

    public static KafkaEvent ToEvent(this string @event)
    {
        if (Enum.TryParse(@event, true, out KafkaEvent result))
        {
            return result;
        }
        return KafkaEvent.Unknown;
    }
}