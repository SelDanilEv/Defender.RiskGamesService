using Defender.RiskGamesService.Application.Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Defender.RiskGamesService.Application.Configuration.Extension;

public static class ServiceOptionsExtensions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WalletOptions>(configuration.GetSection(nameof(WalletOptions)));

        return services;
    }
}