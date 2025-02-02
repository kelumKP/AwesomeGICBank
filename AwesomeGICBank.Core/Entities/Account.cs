using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AwesomeGICBank.Core.Entities
{
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; } = new List<Transaction>();

        public Account(string accountNumber)
        {
            AccountNumber = accountNumber;
            Balance = 0;
        }

        public void Deposit(decimal amount, DateTime date)
        {
            Balance += amount;
            Transactions.Add(new Transaction(date, TransactionType.Deposit, amount));
        }

        public void Withdraw(decimal amount, DateTime date)
        {
            if (Balance < amount)
                throw new InvalidOperationException("Insufficient balance.");
            Balance -= amount;
            Transactions.Add(new Transaction(date, TransactionType.Withdrawal, amount));
        }
    }
}
