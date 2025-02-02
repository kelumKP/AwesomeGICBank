﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Core.Entities
{
    public class Transaction
    {
        public DateTime Date { get; }
        public string TransactionId { get; }
        public TransactionType Type { get; }
        public decimal Amount { get; }

        public Transaction(DateTime date, TransactionType type, decimal amount)
        {
            Date = date;
            Type = type;
            Amount = amount;
            TransactionId = $"{date:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 2)}";
        }
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal
    }
}
