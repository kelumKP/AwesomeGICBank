using AwesomeGICBank.Core.Entities;
using AwesomeGICBank.Core.Interfaces.AwesomeGICBank.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly Dictionary<string, Account> _accounts = new Dictionary<string, Account>();

        public Account FindAccount(string accountNumber)
        {
            _accounts.TryGetValue(accountNumber, out var account);
            return account;
        }

        public Account FindOrCreateAccount(string accountNumber)
        {
            if (!_accounts.ContainsKey(accountNumber))
                _accounts[accountNumber] = new Account(accountNumber);
            return _accounts[accountNumber];
        }
    }
}
