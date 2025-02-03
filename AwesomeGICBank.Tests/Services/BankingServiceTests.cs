// AwesomeGICBank.Tests/BankingServiceTests.cs
using NUnit.Framework;
using AwesomeGICBank.Core.Services;
using AwesomeGICBank.Entities;
using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Interfaces;
using AwesomeGICBank.Infrastructure.Repositories;
using SQLitePCL;

namespace AwesomeGICBank.Tests.Services
{
    public class BankingServiceTests
    {
        private BankingService _bankingService;
        private IAccountRepository _accountRepository;
        private IInterestRuleRepository _interestRuleRepository;

        [SetUp]
        public void Setup()
        {
            Batteries_V2.Init(); // Correct SQLite initialization

            _accountRepository = new AccountRepository();
            _interestRuleRepository = new InterestRuleRepository();
            _bankingService = new BankingService(_accountRepository, _interestRuleRepository);

            // Ensure the account exists before transactions
            var accountNumber = "AC001";
            var account = _accountRepository.FindOrCreateAccount(accountNumber);
        }

        [Test]
        public void CalculateInterest_WithMultipleRules_CalculatesCorrectInterest()
        {
            var accountNumber = "AC001";
            var date1 = DateTime.ParseExact("20230601", "yyyyMMdd", null);
            var date2 = DateTime.ParseExact("20230615", "yyyyMMdd", null);
            var date3 = DateTime.ParseExact("20230626", "yyyyMMdd", null);

            _bankingService.ProcessTransaction(accountNumber, date1, TransactionType.Deposit, 250.00m);
            _bankingService.ProcessTransaction(accountNumber, date3, TransactionType.Withdrawal, 120.00m);

            var transactions = _bankingService.GetAccountTransactions(accountNumber);
            Assert.IsNotEmpty(transactions, "Transactions were not saved to the database.");

            _interestRuleRepository.AddOrUpdateRule(new InterestRule(date1, "RULE02", 1.90m));
            _interestRuleRepository.AddOrUpdateRule(new InterestRule(date2, "RULE03", 2.20m));

            var interest = _bankingService.CalculateInterest(accountNumber, 2024, 2);
            Assert.AreEqual(0.39m, interest);
        }

    }
}