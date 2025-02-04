﻿using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Interfaces;
using AwesomeGICBank.Core.Services;
using NUnit.Framework;
using AwesomeGICBank.Entities;

namespace AwesomeGICBank.Tests.Services
{
    [TestFixture]
    public class BankingServiceTests
    {
        private Mock<IAccountRepository> _mockAccountRepository;
        private Mock<IInterestRuleRepository> _mockInterestRuleRepository;
        private Mock<ITransactionRepository> _mockTransactionRepository;
        private BankingService _bankingService;

        [SetUp]
        public void Setup()
        {
            // Initialize mocks
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockInterestRuleRepository = new Mock<IInterestRuleRepository>();
            _mockTransactionRepository = new Mock<ITransactionRepository>();

            // Initialize the service with mocks
            _bankingService = new BankingService(
                _mockAccountRepository.Object,
                _mockInterestRuleRepository.Object,
                _mockTransactionRepository.Object
            );
        }

        [Test]
        public void ProcessTransaction_Deposit_ShouldUpdateBalance()
        {
            // Arrange
            string accountNumber = "12345";
            DateTime transactionDate = DateTime.Now;
            decimal depositAmount = 100;
            var account = new Account(accountNumber);
            _mockAccountRepository.Setup(repo => repo.FindOrCreateAccount(accountNumber)).Returns(account);

            // Act
            _bankingService.ProcessTransaction(accountNumber, transactionDate, TransactionType.D, depositAmount);

            // Assert
            Assert.AreEqual(100, account.Balance);
            _mockAccountRepository.Verify(repo => repo.AddTransaction(accountNumber, transactionDate, TransactionType.D, depositAmount), Times.Once);
        }

        [Test]
        public void ProcessTransaction_Withdraw_ShouldUpdateBalance()
        {
            // Arrange
            string accountNumber = "12345";
            DateTime transactionDate = DateTime.Now;
            decimal depositAmount = 100;
            decimal withdrawAmount = 50;
            var account = new Account(accountNumber);
            account.Deposit(depositAmount, transactionDate);
            _mockAccountRepository.Setup(repo => repo.FindOrCreateAccount(accountNumber)).Returns(account);

            // Act
            _bankingService.ProcessTransaction(accountNumber, transactionDate, TransactionType.W, withdrawAmount);

            // Assert
            Assert.AreEqual(50, account.Balance);
            _mockAccountRepository.Verify(repo => repo.AddTransaction(accountNumber, transactionDate, TransactionType.W, withdrawAmount), Times.Once);
        }

        [Test]
        public void ProcessTransaction_Withdraw_InsufficientBalance_ShouldThrowException()
        {
            // Arrange
            string accountNumber = "12345";
            DateTime transactionDate = DateTime.Now;
            decimal withdrawAmount = 100;
            var account = new Account(accountNumber);
            _mockAccountRepository.Setup(repo => repo.FindOrCreateAccount(accountNumber)).Returns(account);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _bankingService.ProcessTransaction(accountNumber, transactionDate, TransactionType.W, withdrawAmount));

            Assert.AreEqual("Insufficient balance.", exception.Message);
        }

        [Test]
        public async Task GetTransactionsForAccount_ShouldReturnTransactions()
        {
            // Arrange
            string accountNumber = "12345";
            var transactions = new List<Transaction>
            {
                new Transaction("txn1", DateTime.Now, TransactionType.D, 100, 100),
                new Transaction("txn2", DateTime.Now, TransactionType.W, 50, 50)
            };

            _mockTransactionRepository.Setup(repo => repo.GetAllTransactionsForAccount(accountNumber))
                .ReturnsAsync(transactions);

            // Act
            var result = await _bankingService.GetTransactionsForAccount(accountNumber);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("txn1", result[0].TransactionId);
            Assert.AreEqual("txn2", result[1].TransactionId);
        }
    }
}
