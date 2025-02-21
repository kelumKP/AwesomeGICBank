using System;
using System.Collections.Generic;
using AwesomeGICBank.Core.Entities;

namespace AwesomeGICBank.Infrastructure
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllTransactionsForAccount(string accountNumber);
        Task<List<Transaction>> GetAllTransactionsForAccountPeriod(string accountNumber, DateTime startDate, DateTime endDate);
    }
}
