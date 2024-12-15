using Defender.Kafka.BackgroundServices;
using Defender.Kafka.Configuration.Options;
using Defender.Kafka.Service;
using Defender.RiskGamesService.Common.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Defender.RiskGamesService.Application.Services.Background.Kafka;

public class CreateKafkaTopicsService(
    IKafkaTopicNameResolver kafkaTopicNameResolver,
    IOptions<KafkaOptions> kafkaOptions,
    ILogger<CreateKafkaTopicsService> logger)
    : EnsureTopicsCreatedService(kafkaOptions, logger)
{
    protected override IEnumerable<string> Topics =>
        [
            kafkaTopicNameResolver.ResolveTopicName(KafkaTopic.ScheduledTasks.GetName()),
            kafkaTopicNameResolver.ResolveTopicName(KafkaTopic.LotteryToProcess.GetName()),
        ];

    protected override short ReplicationFactor => 1;

    protected override int NumPartitions => 3;
}
