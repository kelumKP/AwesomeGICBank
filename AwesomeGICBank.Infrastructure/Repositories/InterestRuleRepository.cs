using AwesomeGICBank.Core.Interfaces;
using AwesomeGICBank.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Infrastructure.Repositories
{
    public class InterestRuleRepository : IInterestRuleRepository
    {
        private readonly List<InterestRule> _rules = new List<InterestRule>();

        public void AddOrUpdateRule(InterestRule rule)
        {
            // Remove existing rule for the same date (if any)
            _rules.RemoveAll(r => r.Date == rule.Date);
            _rules.Add(rule);
        }

        public List<InterestRule> GetAllRules()
        {
            return _rules;
        }
    }
}
