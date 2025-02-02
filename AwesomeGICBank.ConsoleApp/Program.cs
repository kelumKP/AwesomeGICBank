// AwesomeGICBank.ConsoleApp/Program.cs
using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Services;
using AwesomeGICBank.Entities;
using AwesomeGICBank.Infrastructure.Repositories;

namespace AwesomeGICBank.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountRepository = new AccountRepository();
            var interestRuleRepository = new InterestRuleRepository();
            var bankingService = new BankingService(accountRepository, interestRuleRepository);
            var interestRuleService = new InterestRuleService(interestRuleRepository);

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
                        InputTransaction(bankingService);
                        break;
                    case "I":
                        DefineInterestRule(interestRuleService);
                        break;
                    case "P":
                        PrintStatement(bankingService);
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

        static void InputTransaction(BankingService bankingService)
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
                var type = parts[2].ToUpper() == "D" ? TransactionType.Deposit : TransactionType.Withdrawal;
                var amount = decimal.Parse(parts[3]);

                bankingService.ProcessTransaction(accountNumber, date, type, amount);
                Console.WriteLine("Transaction processed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void DefineInterestRule(InterestRuleService interestRuleService)
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

                interestRuleService.AddOrUpdateInterestRule(date, ruleId, rate);
                Console.WriteLine("Interest rule added/updated successfully.");

                // Display all interest rules
                var rules = interestRuleService.GetAllInterestRules();
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

        static void PrintStatement(BankingService bankingService)
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
                var year = int.Parse(yearMonth.Substring(0, 4));
                var month = int.Parse(yearMonth.Substring(4, 2));

                var transactions = bankingService.GetAccountTransactions(accountNumber)
                                                .Where(t => t.Date.Year == year && t.Date.Month == month)
                                                .OrderBy(t => t.Date)
                                                .ToList();

                var interest = bankingService.CalculateInterest(accountNumber, year, month);

                Console.WriteLine($"Account: {accountNumber}");
                Console.WriteLine("| Date     | Txn Id      | Type | Amount | Balance |");
                decimal balance = 0;
                foreach (var txn in transactions)
                {
                    balance += txn.Type == TransactionType.Deposit ? txn.Amount : -txn.Amount;
                    Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.TransactionId} | {txn.Type} | {txn.Amount,7:F2} | {balance,7:F2} |");
                }

                if (interest > 0)
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

        static void Quit()
        {
            Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
            Console.WriteLine("Have a nice day!");
        }
    }
}