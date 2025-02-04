using AwesomeGICBank.Core.Entities;
using System;
using System.Collections.Generic;

namespace AwesomeGICBank.Infrastructure
{
    public interface IAccountRepository
    {
        Account FindAccount(string accountNumber);
        Account FindOrCreateAccount(string accountNumber);
        void AddTransaction(string accountNumber, DateTime date, TransactionType type, decimal amount);
        List<Transaction> GetTransactionsForAccount(string accountNumber);
    }
}