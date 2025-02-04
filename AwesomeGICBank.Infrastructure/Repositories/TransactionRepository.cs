using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwesomeGICBank.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private const string ConnectionString = "Data Source=awesomegicbank.db";

        public TransactionRepository()
        {
            InitializeDatabase();
        }

        public Task<List<Transaction>> GetAllTransactionsForAccount(string accountNumber, DateTime startDate, DateTime endDate)
        {
            var transactions = new List<Transaction>();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT TransactionId, Date, Type, Amount, EODBalance 
                    FROM Transactions 
                    WHERE AccountNumber = @AccountNumber 
                    AND Date BETWEEN @StartDate AND @EndDate
                    ORDER BY Date";
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);
                command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyyMMdd"));
                command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyyMMdd"));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) // No need for async here
                    {
                        var transactionId = reader.GetString(0);
                        var date = DateTime.ParseExact(reader.GetString(1), "yyyyMMdd", null);
                        var typeString = reader.GetString(2);
                        var amount = reader.GetDecimal(3);
                        var eodBalance = reader.GetDecimal(4);

                        TransactionType type = typeString == "D" ? TransactionType.D : TransactionType.W;

                        transactions.Add(new Transaction(date, type, amount, eodBalance));
                    }
                }
            }

            return Task.FromResult(transactions); // Wrap the result in Task
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Transactions (
                    TransactionId TEXT PRIMARY KEY,
                    AccountNumber TEXT,
                    Date TEXT NOT NULL,
                    Type TEXT NOT NULL,
                    Amount DECIMAL NOT NULL,
                    EODBalance DECIMAL NOT NULL, -- New column
                    FOREIGN KEY (AccountNumber) REFERENCES Accounts(AccountNumber)
                );";
                command.ExecuteNonQuery();
            }
        }

    }
}
