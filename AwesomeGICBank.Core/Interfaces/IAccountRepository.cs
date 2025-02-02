using AwesomeGICBank.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Core.Interfaces
{
    namespace AwesomeGICBank.Core.Interfaces
    {
        public interface IAccountRepository
        {
            Account FindAccount(string accountNumber);
            Account FindOrCreateAccount(string accountNumber);
        }
    }
}
