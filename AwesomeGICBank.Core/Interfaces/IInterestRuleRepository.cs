using AwesomeGICBank.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Core.Interfaces
{
    public interface IInterestRuleRepository
    {
        void AddOrUpdateRule(InterestRule rule);
        List<InterestRule> GetAllRules();
    }
}
