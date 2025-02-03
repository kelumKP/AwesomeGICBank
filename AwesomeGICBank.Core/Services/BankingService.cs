using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Interfaces;
using System.Linq;

namespace AwesomeGICBank.Core.Services
{
    public class BankingService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IInterestRuleRepository _interestRuleRepository;

        public BankingService(IAccountRepository accountRepository, IInterestRuleRepository interestRuleRepository)
        {
            _accountRepository = accountRepository;
            _interestRuleRepository = interestRuleRepository;
        }

        public void ProcessTransaction(string accountNumber, DateTime date, TransactionType type, decimal amount)
        {
            var account = _accountRepository.FindOrCreateAccount(accountNumber);

            if (type == TransactionType.Deposit)
            {
                account.Deposit(amount, date); // Add the amount to the balance
            }
            else if (type == TransactionType.Withdrawal)
            {
                account.Withdraw(amount, date); // Subtract the amount from the balance
            }
            else
            {
                throw new ArgumentException("Invalid transaction type. Use 'D' for deposit or 'W' for withdrawal.");
            }

            // Save the updated account balance and transaction
            _accountRepository.AddTransaction(accountNumber, date, type, amount);
        }

        public List<Transaction> GetAccountTransactions(string accountNumber)
        {
            return _accountRepository.GetTransactionsForAccount(accountNumber);
        }

        public decimal CalculateInterest(string accountNumber, int year, int month)
        {
            var account = _accountRepository.FindAccount(accountNumber);
            if (account == null)
                throw new ArgumentException("Account not found.");

            var rules = _interestRuleRepository.GetAllRules()
                                              .OrderBy(r => r.Date)
                                              .ToList();

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            decimal totalInterest = 0;
            var currentDate = startDate;
            var currentBalance = account.Transactions
                                       .Where(t => t.Date < startDate)
                                       .Sum(t => t.Type == TransactionType.Deposit ? t.Amount : -t.Amount);

            while (currentDate <= endDate)
            {
                // Apply transactions for the current day
                var transactionsForDay = account.Transactions
                                               .Where(t => t.Date.Date == currentDate.Date)
                                               .OrderBy(t => t.Date)
                                               .ToList();

                foreach (var txn in transactionsForDay)
                {
                    if (txn.Type == TransactionType.Deposit)
                        currentBalance += txn.Amount;
                    else
                        currentBalance -= txn.Amount;
                }

                // Determine the applicable interest rule for the current day
                var ruleForDay = rules.LastOrDefault(r => r.Date <= currentDate);
                if (ruleForDay != null)
                {
                    // Calculate daily interest
                    var dailyInterest = currentBalance * (ruleForDay.Rate / 100) / 365;
                    totalInterest += dailyInterest;
                }

                currentDate = currentDate.AddDays(1);
            }

            return Math.Round(totalInterest, 2, MidpointRounding.AwayFromZero);
        }
    }
}