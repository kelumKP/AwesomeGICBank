using AwesomeGICBank.Application.DTOs;
using AwesomeGICBank.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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