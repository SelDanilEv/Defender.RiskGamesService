using System.Reflection;
using Defender.Common.Configuration.Options.Kafka;
using Defender.Common.Extension.Kafka;
using Defender.Common.Extensions;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;
using Defender.RiskGamesService.Application.Factories.Transaction;
using Defender.RiskGamesService.Application.Handlers.Transaction;
using Defender.RiskGamesService.Application.Services.Background;
using Defender.RiskGamesService.Application.Services.Background.Kafka;
using Defender.RiskGamesService.Application.Services.Lottery;
using Defender.RiskGamesService.Application.Services.Lottery.Tickets;
using Defender.RiskGamesService.Application.Services.Transaction;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Defender.RiskGamesService.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.RegisterKafkaServices(configuration)
            .RegisterServices()
            .RegisterBackgroundServices()
            .RegisterFactory();

        return services;
    }

    private static IServiceCollection RegisterKafkaServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(configuration.GetSection(nameof(KafkaOptions)));

        services.AddJsonSerializer();

        services.AddDefaultKafkaConsumer();
        services.AddDefaultKafkaProducer();
        
        services.AddHostedService<CreateKafkaTopicsService>();

        services.AddHostedService<EventListenerService>();

        return services;
    }

    private static IServiceCollection RegisterBackgroundServices(
        this IServiceCollection services)
    {
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

        services.AddSingleton<LotteryTransactionHandler>();
        services.AddSingleton<StartRechargeTransactionHandler>();
        services.AddSingleton<StartPaymentTransactionHandler>();

        return services;
    }
}
