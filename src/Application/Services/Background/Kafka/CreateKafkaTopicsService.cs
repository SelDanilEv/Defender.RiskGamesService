using Defender.Common.Configuration.Options.Kafka;
using Defender.Common.Kafka.BackgroundServices;
using Defender.RiskGamesService.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Defender.RiskGamesService.Application.Services.Background.Kafka;

public class CreateKafkaTopicsService(
    IOptions<KafkaOptions> kafkaOptions,
    ILogger<CreateKafkaTopicsService> logger)
    : EnsureTopicsCreatedService(kafkaOptions, logger)
{
    protected override IEnumerable<string> Topics =>
        [
            KafkaTopic.ScheduledTasks.GetName(),
            KafkaTopic.LotteryToProcess.GetName()
        ];

    protected override short ReplicationFactor => 1;

    protected override int NumPartitions => 3;
}
