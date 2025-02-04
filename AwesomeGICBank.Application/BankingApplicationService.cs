using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Services;
using AwesomeGICBank.Infrastructure.Repositories;
using SQLitePCL;
using AwesomeGICBank.Application.DTOs;

namespace AwesomeGICBank.Application
{
    public class BankingApplicationService
    {
        private readonly BankingService _bankingService;
        private readonly InterestRuleService _interestRuleService;

        public BankingApplicationService()
        {
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
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private void InputTransaction()
        {
            Console.WriteLine("Enter transaction details <Date YYYYMMDD> <Account> <D/W> <Amount>:");
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Invalid input format.");
                return;
            }

            try
            {
                var transactionDto = new TransactionInputDto
                {
                    Date = DateTime.ParseExact(parts[0], "yyyyMMdd", null),
                    AccountNumber = parts[1],
                    Type = parts[2].ToUpper() == "D" ? TransactionType.D : TransactionType.W,
                    Amount = decimal.Parse(parts[3])
                };

                _bankingService.ProcessTransaction(transactionDto.AccountNumber, transactionDto.Date, transactionDto.Type, transactionDto.Amount);
                Console.WriteLine("Transaction processed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DefineInterestRule()
        {
            Console.WriteLine("Enter interest rule <Date YYYYMMDD> <RuleId> <Rate %>:");
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Invalid input format.");
                return;
            }

            try
            {
                var interestRuleDto = new InterestRuleInputDto
                {
                    Date = DateTime.ParseExact(parts[0], "yyyyMMdd", null),
                    RuleId = parts[1],
                    Rate = decimal.Parse(parts[2])
                };

                _interestRuleService.AddOrUpdateInterestRule(interestRuleDto.Date, interestRuleDto.RuleId, interestRuleDto.Rate);
                Console.WriteLine("Interest rule added/updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task PrintStatement()
        {
            Console.WriteLine("Enter account and statement period <Account> <YYYYMM>:");
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid input format.");
                return;
            }

            try
            {
                var statementDto = new StatementRequestDto
                {
                    AccountNumber = parts[0],
                    Year = int.Parse(parts[1].Substring(0, 4)),
                    Month = int.Parse(parts[1].Substring(4, 2))
                };

                var transactions = await _bankingService.GetTransactionsForAccount(statementDto.AccountNumber);
                var interest = await _bankingService.CalculateInterest(statementDto.AccountNumber, statementDto.Year, statementDto.Month);

                Console.WriteLine($"Statement for Account: {statementDto.AccountNumber}");
                Console.WriteLine("| Date     | Txn Id      | Type | Amount | Balance |");

                decimal balance = 0;
                foreach (var txn in transactions)
                {
                    balance += txn.Type == TransactionType.D ? txn.Amount : -txn.Amount;
                    Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.TransactionId} | {txn.Type} | {txn.Amount,7:F2} | {balance,7:F2} |");
                }

                if (interest > 0.0m)
                {
                    balance += interest;
                    Console.WriteLine($"| {new DateTime(statementDto.Year, statementDto.Month, DateTime.DaysInMonth(statementDto.Year, statementDto.Month)):yyyyMMdd} |             | I    | {interest,7:F2} | {balance,7:F2} |");
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
