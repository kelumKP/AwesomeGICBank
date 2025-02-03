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
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.");

            Balance += amount; // Add the amount to the balance
            Transactions.Add(new Transaction(date, TransactionType.D, amount));
        }

        public void Withdraw(decimal amount, DateTime date)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.");

            if (Balance < amount)
                throw new InvalidOperationException("Insufficient balance.");

            Balance -= amount; // Subtract the amount from the balance
            Transactions.Add(new Transaction(date, TransactionType.W, amount));
        }
    }
}
