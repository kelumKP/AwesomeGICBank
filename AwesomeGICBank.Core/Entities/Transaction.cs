using System;
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
        public decimal EODBalance { get; }

        // Modify constructor to accept TransactionId as parameter
        public Transaction(string transactionId, DateTime date, TransactionType type, decimal amount, decimal eodBalance)
        {
            TransactionId = transactionId; // Set the TransactionId from DB
            Date = date;
            Type = type;
            Amount = amount;
            EODBalance = eodBalance;
        }
    }




    public enum TransactionType
    {
        D,
        W
    }
}
