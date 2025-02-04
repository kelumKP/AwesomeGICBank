using AwesomeGICBank.Application.Interfaces;
using AwesomeGICBank.Infrastructure.Repositories;
using AwesomeGICBank.Infrastructure;
using Microsoft.Extensions.DependencyInjection;


namespace AwesomeGICBank.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAwesomeGICBankServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<ITransactionRepository, TransactionRepository>();
            services.AddSingleton<IInterestRuleRepository, InterestRuleRepository>();

            // Register application services
            services.AddSingleton<IBankingService, BankingService>();
            services.AddSingleton<IInterestRuleService, InterestRuleService>();

            // Register the top-level service
            services.AddSingleton<BankingApplicationService>();

            return services;
        }
    }
}
