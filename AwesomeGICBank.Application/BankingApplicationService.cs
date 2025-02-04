using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Services;
using AwesomeGICBank.Infrastructure.Repositories;
using SQLitePCL;

namespace AwesomeGICBank.Application
{
    public class BankingApplicationService
    {
        private readonly BankingService _bankingService;
        private readonly InterestRuleService _interestRuleService;

        public BankingApplicationService()
        {
            // Initialize SQLite
            Batteries.Init();

            var accountRepository = new AccountRepository();
            var interestRuleRepository = new InterestRuleRepository();
            var transactionRepository = new TransactionRepository();
            _bankingService = new BankingService(accountRepository, interestRuleRepository, transactionRepository);
            _interestRuleService = new InterestRuleService(interestRuleRepository);
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
                Console.WriteLine("[T] Input transactions");
                Console.WriteLine("[I] Define interest rules");
                Console.WriteLine("[P] Print statement");
                Console.WriteLine("[Q] Quit");
                Console.Write("> ");

                var input = Console.ReadLine()?.ToUpper();

                switch (input)
                {
                    case "T":
                        InputTransaction();
                        break;
                    case "I":
                        DefineInterestRule();
                        break;
                    case "P":
                        PrintStatement().Wait();
                        break;
                    case "Q":
                        Quit();
                        return; // Exit the application
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private void InputTransaction()
        {
            Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format (or enter blank to go back to main menu):");
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return;

            var parts = input.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Invalid input format.");
                return;
            }

            try
            {
                var date = DateTime.ParseExact(parts[0], "yyyyMMdd", null);
                var accountNumber = parts[1];
                var type = parts[2].ToUpper() == "D" ? TransactionType.D : TransactionType.W;
                var amount = decimal.Parse(parts[3]);

                _bankingService.ProcessTransaction(accountNumber, date, type, amount);
                Console.WriteLine("Transaction processed successfully.");

                // Fetch and display the transaction history for the account
                var transactions = _bankingService.GetAccountTransactions(accountNumber)
                                                .OrderBy(t => t.Date)
                                                .ToList();

                Console.WriteLine($"Account: {accountNumber}");
                Console.WriteLine("| Date     | Txn Id      | Type | Amount |");
                foreach (var txn in transactions)
                {
                    Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.TransactionId} | {txn.Type} | {txn.Amount,7:F2} |");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DefineInterestRule()
        {
            Console.WriteLine("Please enter interest rules details in <Date> <RuleId> <Rate in %> format (or enter blank to go back to main menu):");
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return;

            var parts = input.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Invalid input format.");
                return;
            }

            try
            {
                var date = DateTime.ParseExact(parts[0], "yyyyMMdd", null);
                var ruleId = parts[1];
                var rate = decimal.Parse(parts[2]);

                _interestRuleService.AddOrUpdateInterestRule(date, ruleId, rate);
                Console.WriteLine("Interest rule added/updated successfully.");

                // Display all interest rules
                var rules = _interestRuleService.GetAllInterestRules();
                Console.WriteLine("Interest rules:");
                Console.WriteLine("| Date     | RuleId | Rate (%) |");
                foreach (var rule in rules)
                {
                    Console.WriteLine($"| {rule.Date:yyyyMMdd} | {rule.RuleId} | {rule.Rate,8:F2} |");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task PrintStatement()
        {
            Console.WriteLine("Please enter account and month to generate the statement <Account> <Year><Month> (or enter blank to go back to main menu):");
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return;

            var parts = input.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid input format.");
                return;
            }

            try
            {
                var accountNumber = parts[0];
                var yearMonth = parts[1];

                // Validate the format of yearMonth
                if (yearMonth.Length != 6 || !int.TryParse(yearMonth, out _))
                {
                    Console.WriteLine("Invalid YearMonth format.");
                    return;
                }

                var year = int.Parse(yearMonth.Substring(0, 4));
                var month = int.Parse(yearMonth.Substring(4, 2));

                if (month < 1 || month > 12)
                {
                    Console.WriteLine("Invalid month value.");
                    return;
                }

                var transactions = await _bankingService.GetTransactionsForAccount(accountNumber);

                var interest = await _bankingService.CalculateInterest(accountNumber, year, month);

                Console.WriteLine($"Account: {accountNumber}");
                Console.WriteLine("| Date     | Txn Id      | Type | Amount | Balance |");

                decimal balance = 0;
                foreach (var txn in transactions)
                {
                    // Update balance depending on transaction type
                    balance += txn.Type == TransactionType.D ? txn.Amount : -txn.Amount;

                    // Print transaction details
                    Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.TransactionId} | {txn.Type} | {txn.Amount,7:F2} | {balance,7:F2} |");
                }

                // Apply interest after all transactions
                if (interest > 0.0m)
                {
                    balance += interest;
                    Console.WriteLine($"| {new DateTime(year, month, DateTime.DaysInMonth(year, month)):yyyyMMdd} |             | I    | {interest,7:F2} | {balance,7:F2} |");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void Quit()
        {
            Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
            Console.WriteLine("Have a nice day!");
        }
    }
}