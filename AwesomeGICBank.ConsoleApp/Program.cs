using Microsoft.Extensions.DependencyInjection;
using AwesomeGICBank.Application;
using AwesomeGICBank.Infrastructure.Repositories;
using AwesomeGICBank.Infrastructure;
using AwesomeGICBank.Application.Interfaces;

class Program
{
    static void Main(string[] args)
    {
        // Set up the DI container
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IAccountRepository, AccountRepository>()
            .AddSingleton<IInterestRuleRepository, InterestRuleRepository>()
            .AddSingleton<ITransactionRepository, TransactionRepository>()
            .AddSingleton<IBankingService, BankingService>()
            .AddSingleton<IInterestRuleService, InterestRuleService>()
            .AddSingleton<BankingApplicationService>()
            .BuildServiceProvider();

        // Resolve the BankingApplicationService from the DI container
        var bankingAppService = serviceProvider.GetService<BankingApplicationService>();
        bankingAppService.Start();
    }
}