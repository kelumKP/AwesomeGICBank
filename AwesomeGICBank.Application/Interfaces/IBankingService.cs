using AwesomeGICBank.Application.DTOs;
using AwesomeGICBank.Core.Entities;

namespace AwesomeGICBank.Application.Interfaces
{
    public interface IBankingService
    {
        void ProcessTransaction(TransactionInputDto transactionDto);
        List<Transaction> GetAccountTransactions(string accountNumber);
        Task<List<Transaction>> GetTransactionsForAccount(string accountNumber);
        Task<decimal> CalculateInterest(string accountNumber, int year, int month);
        List<dynamic> GetApplicableRules(string accountNumber, int year, int month);
        Task<List<dynamic>> GetEODBalanceExistedPeriods(string accountNumber, int year, int month);
        Task<List<dynamic>> GetApplicableInterestPeriods(string accountNumber, int year, int month);
    }
}