using System.Reflection;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Defender.RiskGamesService.Application.Services.Background;
using Defender.RiskGamesService.Application.Services.Lottery.Background;
using Defender.RiskGamesService.Application.Services.Lottery.Tickets;
using Defender.RiskGamesService.Application.Services.Lottery;
using Defender.RiskGamesService.Application.Services.Transaction;
using Defender.RiskGamesService.Application.Handlers.Transaction;
using Defender.RiskGamesService.Application.Factories.Transaction;

namespace Defender.RiskGamesService.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services
            .RegisterServices()
            .RegisterBackgroundServices()
            .RegisterFactory();

        return services;
    }



    private static IServiceCollection RegisterBackgroundServices(
        this IServiceCollection services)
    {
        services.AddHostedService<LotteryScheduleAndProcessingService>();
        services.AddHostedService<TransactionStatusesListenerService>();

        return services;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<ILotteryManagementService, LotteryManagementService>();
        services.AddTransient<ILotteryProcessingService, LotteryProcessingService>();
        services.AddTransient<IUserTicketManagementService, UserTicketManagementService>();

        services.AddTransient<ITransactionManagementService, TransactionManagementService>();

        return services;
    }

    private static IServiceCollection RegisterFactory(this IServiceCollection services)
    {
        services.AddSingleton<TransactionHandlerFactory>();

        services.AddScoped<LotteryTransactionHandler>();
        services.AddScoped<StartRechargeTransactionHandler>();
        services.AddScoped<StartPaymentTransactionHandler>();

        return services;
    }
}
