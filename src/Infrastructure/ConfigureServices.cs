using System.Reflection;
using Defender.RiskGamesService.Infrastructure.Repositories.Lottery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Defender.Common.Clients.Wallet;
using Microsoft.Extensions.Options;
using Defender.RiskGamesService.Application.Configuration.Options;
using Defender.RiskGamesService.Infrastructure.Clients.Wallet;
using Defender.RiskGamesService.Infrastructure.Repositories.Transactions;
using Defender.Common.DB.SharedStorage.Entities;
using Defender.Mongo.MessageBroker.Extensions;
using Microsoft.Extensions.Hosting;
using Defender.Common.Extension;
using Defender.Common.DB.SharedStorage.MessageBroker;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Transactions;
using Defender.RiskGamesService.Application.Common.Interfaces.Wrapper;

namespace Defender.RiskGamesService.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services
            .RegisterRepositories()
            .RegisterApiClients(configuration)
            .RegisterClientWrappers()
            .RegisterMessageBroker(environment);

        return services;
    }

    private static IServiceCollection RegisterClientWrappers(this IServiceCollection services)
    {
        services.AddTransient<IWalletWrapper, WalletWrapper>();

        return services;
    }

    private static IServiceCollection RegisterMessageBroker(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddTopicConsumer<TransactionStatusUpdatedEvent>(opt =>
        {
            opt.ApplyOptions(new TransactionStatusesTopicConsumerOptions(environment.GetAppEnvironment()));
        });

        return services;
    }

    private static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ILotteryRepository, LotteryRepository>();
        services.AddSingleton<ILotteryDrawRepository, LotteryDrawRepository>();
        services.AddSingleton<ILotteryUserTicketRepository, LotteryUserTicketRepository>();

        services.AddSingleton<ITransactionToTrackRepository, TransactionToTrackRepository>();
        services.AddSingleton<IOutboxTransactionStatusRepository, OutboxTransactionStatusRepository>();

        return services;
    }

    private static IServiceCollection RegisterApiClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.RegisterWalletClient(
            (serviceProvider, client) =>
            {
                client.BaseAddress = new Uri(
                    serviceProvider.GetRequiredService<IOptions<WalletOptions>>().Value.Url);
            });

        return services;
    }

}
