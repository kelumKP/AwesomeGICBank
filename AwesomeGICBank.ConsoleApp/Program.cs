﻿using Microsoft.Extensions.DependencyInjection;
using AwesomeGICBank.Application;

class Program
{
    static void Main(string[] args)
    {
        // Set up the DI container using the extension method
        var serviceProvider = new ServiceCollection()
            .AddAwesomeGICBankServices() // Call the extension method to register all dependencies
            .BuildServiceProvider();

        // Resolve the BankingApplicationService from the DI container
        var bankingAppService = serviceProvider.GetService<BankingApplicationService>();
        bankingAppService.Start();
    }
}