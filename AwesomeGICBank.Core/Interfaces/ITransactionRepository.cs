﻿using System;
using System.Collections.Generic;
using AwesomeGICBank.Core.Entities; // Make sure to include this namespace

namespace AwesomeGICBank.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllTransactionsForAccount(string accountNumber);
        Task<List<Transaction>> GetAllTransactionsForAccountPeriod(string accountNumber, DateTime startDate, DateTime endDate);
    }
}
