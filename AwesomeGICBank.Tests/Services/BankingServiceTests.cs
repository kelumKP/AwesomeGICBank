using NUnit.Framework;
using AwesomeGICBank.Core.Services;
using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Interfaces;
using AwesomeGICBank.Infrastructure.Repositories;
using System;
using System.Linq;
using AwesomeGICBank.Entities;
using SQLitePCL;

namespace AwesomeGICBank.Tests.Services
{
    public class BankingServiceTests
    {
        private BankingService _bankingService;
        private IAccountRepository _accountRepository;
        private IInterestRuleRepository _interestRuleRepository;
        private ITransactionRepository _transactionRepository;

        [SetUp]
        public void Setup()
        {
            // Initialize SQLite and repositories
            Batteries_V2.Init(); // Initialize SQLite

            _accountRepository = new AccountRepository();
            _interestRuleRepository = new InterestRuleRepository();
            _transactionRepository = new TransactionRepository(); // Initialize transaction repository
            _bankingService = new BankingService(_accountRepository, _interestRuleRepository, _transactionRepository);

            // Ensure the account exists before transactions
            var accountNumber = "AC001";
            _accountRepository.FindOrCreateAccount(accountNumber);
        }

        [Test]
        public async Task CalculateInterest_WithMultipleRules_CalculatesCorrectInterest()
        {
            var accountNumber = "AC001";
            var date1 = DateTime.ParseExact("20230601", "yyyyMMdd", null);
            var date2 = DateTime.ParseExact("20230615", "yyyyMMdd", null);
            var date3 = DateTime.ParseExact("20230626", "yyyyMMdd", null);

            // Process transactions
            _bankingService.ProcessTransaction(accountNumber, date1, TransactionType.D, 250.00m); // Deposit
            _bankingService.ProcessTransaction(accountNumber, date3, TransactionType.W, 120.00m); // Withdrawal

            var transactions = _bankingService.GetAccountTransactions(accountNumber);
            Assert.IsNotEmpty(transactions, "Transactions were not saved to the database.");

            // Add interest rules (ensure rules are added in correct order and for correct periods)
            _interestRuleRepository.AddOrUpdateRule(new InterestRule(date1, "RULE02", 1.90m)); // 1.90% from date1
            _interestRuleRepository.AddOrUpdateRule(new InterestRule(date2, "RULE03", 2.20m)); // 2.20% from date2

            // Calculate interest for the specified year and month
            var interest = await _bankingService.CalculateInterest(accountNumber, 2023, 6); // Calculate for June 2023

            // Assert that the expected interest is calculated correctly, with a tolerance
            var expectedInterest = 0.39m;
            var tolerance = 0.01m; // Allowing a small tolerance for floating-point comparison

            // Round the actual interest for comparison
            var roundedInterest = Math.Round(interest, 2); // Round to 2 decimal places

            // Use Within to compare the interest value with a tolerance
            Assert.That(roundedInterest, Is.EqualTo(expectedInterest).Within(tolerance), "Interest calculation is incorrect.");
        }


    }
}
