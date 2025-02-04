using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Interfaces;
using System.Linq;
using System.Security.Principal;

namespace AwesomeGICBank.Core.Services
{
    public class BankingService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IInterestRuleRepository _interestRuleRepository;
        private readonly ITransactionRepository _transactionRepository;

        public BankingService(IAccountRepository accountRepository, IInterestRuleRepository interestRuleRepository, ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _interestRuleRepository = interestRuleRepository;
            _transactionRepository = transactionRepository;
        }

        public void ProcessTransaction(string accountNumber, DateTime date, TransactionType type, decimal amount)
        {
            var account = _accountRepository.FindOrCreateAccount(accountNumber);

            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.");

            if (type == TransactionType.D)
            {
                account.Deposit(amount, date);
            }
            else if (type == TransactionType.W)
            {
                account.Withdraw(amount, date);
            }
            else
            {
                throw new ArgumentException("Invalid transaction type. Use 'D' for deposit or 'W' for withdrawal.");
            }

            _accountRepository.AddTransaction(accountNumber, date, type, amount);
        }


        public List<Transaction> GetAccountTransactions(string accountNumber)
        {
            return _accountRepository.GetTransactionsForAccount(accountNumber);
        }

        public async Task<List<Transaction>> GetTransactionsForAccount(string accountNumber)
        {
            // Call the GetAllTransactionsForAccount method from the TransactionRepository
            var transactions = await _transactionRepository.GetAllTransactionsForAccount(accountNumber);

            // Return the list of transactions
            return transactions;
        }

        public async Task<decimal> CalculateInterest(string accountNumber, int year, int month)
        {
            try
            {
                var interestForMonth = await GetApplicableInterestPeriods(accountNumber, year, month);
                decimal totalAnnualizedInterest = interestForMonth.Sum(x => (decimal)x.Annualized_Interest);
                return totalAnnualizedInterest / 365;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating interest: {ex.Message}");
                return 0.0m;  // Return 0 if there's an error
            }
        }

        public List<dynamic> GetApplicableRules(string accountNumber, int year, int month)
        {
            // Simulate the interest rules data
            var rules = _interestRuleRepository.GetAllRules();

            // Get the start and end date of the selected month
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1); // Last day of the month

            // List to hold applicable rules
            var applicableRules = new List<dynamic>();

            // Iterate through the rules to find applicable ones
            for (int i = 0; i < rules.Count; i++)
            {
                var currentRule = rules[i];
                DateTime ruleStartDate = currentRule.Date;

                // Determine the rule's end date
                DateTime ruleEndDate = (i + 1 < rules.Count) ? rules[i + 1].Date.AddDays(-1) : endDate;

                // Calculate the intersection between the rule and the selected month
                DateTime intersectionStart = ruleStartDate > startDate ? ruleStartDate : startDate;
                DateTime intersectionEnd = ruleEndDate < endDate ? ruleEndDate : endDate;

                // Only add the rule if there is an intersection with the selected month
                if (intersectionStart <= intersectionEnd)
                {
                    applicableRules.Add(new
                    {
                        StartDate = intersectionStart,
                        RuleId = currentRule.RuleId,
                        Rate = currentRule.Rate,
                        EndDate = intersectionEnd
                    });
                }
            }

            return applicableRules;
        }

        public async Task<List<dynamic>> GetEODBalanceExistedPeriods(string accountNumber, int year, int month)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // Await the asynchronous method
            var transactions = await _transactionRepository.GetAllTransactionsForAccountPeriod(accountNumber, startDate, endDate);

            var filteredTransactions = transactions
                .Where(txn => txn.Date >= startDate && txn.Date <= endDate)
                .OrderBy(txn => txn.Date)
                .ToList();

            var periods = new List<dynamic>();

            if (!filteredTransactions.Any())
            {
                return periods;
            }

            var previousBalance = filteredTransactions[0].EODBalance;
            var periodStartDate = startDate;

            foreach (var txn in filteredTransactions)
            {
                if (txn.EODBalance != previousBalance)
                {
                    int days = (txn.Date - periodStartDate).Days;
                    if (days > 0)
                    {
                        periods.Add(new
                        {
                            EODBalance = previousBalance,
                            NumberOfDays = days,
                            StartDate = periodStartDate,
                            EndDate = txn.Date.AddDays(-1)
                        });
                    }

                    periodStartDate = txn.Date;
                    previousBalance = txn.EODBalance;
                }
            }

            int finalDays = (endDate - periodStartDate).Days;
            if (finalDays > 0)
            {
                periods.Add(new
                {
                    EODBalance = previousBalance,
                    NumberOfDays = finalDays,
                    StartDate = periodStartDate,
                    EndDate = endDate
                });
            }

            return periods;
        }

        public async Task<List<dynamic>> GetApplicableInterestPeriods(string accountNumber, int year, int month)
        {
            var applicableRules = GetApplicableRules(accountNumber, year, month);
            var eodBalanceExistedPeriods = await GetEODBalanceExistedPeriods(accountNumber, year, month); // Await the Task

            var annualizedInterestWithPeriods = new List<dynamic>();

            foreach (var rule in applicableRules)
            {
                foreach (var balance in eodBalanceExistedPeriods)
                {
                    DateTime start = rule.StartDate > balance.StartDate ? rule.StartDate : balance.StartDate;
                    DateTime end = rule.EndDate < balance.EndDate ? rule.EndDate : balance.EndDate;

                    if (start <= end)
                    {
                        int numberOfDays = (end - start).Days + 1;
                        decimal annualizedInterest = balance.EODBalance * (rule.Rate / 100) * numberOfDays; // Apply formula

                        annualizedInterestWithPeriods.Add(new
                        {
                            StartDate = start,
                            RuleId = rule.RuleId,
                            Rate = rule.Rate,
                            EndDate = end,
                            EODBalance = balance.EODBalance,
                            NumberOfDays = numberOfDays,
                            Annualized_Interest = Math.Round(annualizedInterest, 2) // Round to 2 decimal places
                        });
                    }
                }
            }

            return annualizedInterestWithPeriods;
        }
    }
}