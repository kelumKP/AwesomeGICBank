using AwesomeGICBank.Core.Entities;

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
        // Create a new transaction with the current balance as EODBalance
        Transactions.Add(new Transaction(null, date, TransactionType.D, amount, Balance));
    }

    public void Withdraw(decimal amount, DateTime date)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be greater than zero.");

        if (Balance < amount)
            throw new InvalidOperationException("Insufficient balance.");

        Balance -= amount; // Subtract the amount from the balance
        // Create a new transaction with the current balance as EODBalance
        Transactions.Add(new Transaction(null, date, TransactionType.W, amount, Balance));
    }
}
