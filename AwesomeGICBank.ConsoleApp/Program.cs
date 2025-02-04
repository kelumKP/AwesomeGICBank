using AwesomeGICBank.Application;

namespace AwesomeGICBank.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var bankingAppService = new BankingApplicationService();
            bankingAppService.Start();
        }
    }
}