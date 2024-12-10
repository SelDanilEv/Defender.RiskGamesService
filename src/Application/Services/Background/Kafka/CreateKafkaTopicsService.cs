using Defender.Common.Configuration.Options.Kafka;
using Defender.Common.Extension;
using Defender.Common.Kafka.BackgroundServices;
using Defender.RiskGamesService.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Defender.RiskGamesService.Application.Services.Background.Kafka;

public class CreateKafkaTopicsService(
    IHostEnvironment hostEnvironment,
    IOptions<KafkaOptions> kafkaOptions,
    ILogger<CreateKafkaTopicsService> logger)
    : EnsureTopicsCreatedService(kafkaOptions, logger)
{
    protected override IEnumerable<string> Topics =>
        [
            KafkaTopic.ScheduledTasks.GetName(hostEnvironment.GetAppEnvironment()),
            KafkaTopic.LotteryToProcess.GetName(hostEnvironment.GetAppEnvironment())
        ];

    protected override short ReplicationFactor => 1;

    protected override int NumPartitions => 3;
}
