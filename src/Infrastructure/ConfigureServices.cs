using System.Reflection;
using Defender.Common.Clients.Wallet;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Transactions;
using Defender.RiskGamesService.Application.Common.Interfaces.Wrapper;
using Defender.RiskGamesService.Application.Configuration.Options;
using Defender.RiskGamesService.Infrastructure.Clients.Wallet;
using Defender.RiskGamesService.Infrastructure.Repositories.Lottery;
using Defender.RiskGamesService.Infrastructure.Repositories.Transactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
            .RegisterClientWrappers();

        return services;
    }

    private static IServiceCollection RegisterClientWrappers(this IServiceCollection services)
    {
        services.AddTransient<IWalletWrapper, WalletWrapper>();

        return services;
    }

    private static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ILotteryRepository, LotteryRepository>();
        services.AddSingleton<ILotteryDrawRepository, LotteryDrawRepository>();
        services.AddSingleton<ILotteryUserTicketRepository, LotteryUserTicketRepository>();

        services.AddSingleton<ITransactionToTrackRepository, TransactionToTrackRepository>();

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
