using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Interfaces.AwesomeGICBank.Core.Interfaces;
using Microsoft.Data.Sqlite;

namespace AwesomeGICBank.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private const string ConnectionString = "Data Source=awesomegicbank.db";

        public AccountRepository()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Accounts (
                    AccountNumber TEXT PRIMARY KEY,
                    Balance DECIMAL NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Transactions (
                    TransactionId TEXT PRIMARY KEY,
                    AccountNumber TEXT,
                    Date TEXT NOT NULL,
                    Type TEXT NOT NULL,
                    Amount DECIMAL NOT NULL,
                    FOREIGN KEY (AccountNumber) REFERENCES Accounts(AccountNumber)
                );";
                command.ExecuteNonQuery();
            }
        }

        public Account FindAccount(string accountNumber)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT AccountNumber, Balance FROM Accounts WHERE AccountNumber = @AccountNumber";
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var account = new Account(reader.GetString(0));
                        account.Deposit(reader.GetDecimal(1), DateTime.Now); // Set initial balance
                        return account;
                    }
                }
            }
            return null;
        }

        public Account FindOrCreateAccount(string accountNumber)
        {
            var account = FindAccount(accountNumber);
            if (account == null)
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Accounts (AccountNumber, Balance) VALUES (@AccountNumber, 0)";
                    command.Parameters.AddWithValue("@AccountNumber", accountNumber);
                    command.ExecuteNonQuery();
                }
                account = new Account(accountNumber);
            }
            return account;
        }
    }
}