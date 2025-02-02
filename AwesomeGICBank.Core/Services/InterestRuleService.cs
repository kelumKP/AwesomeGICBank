using AwesomeGICBank.Core.Interfaces;
using AwesomeGICBank.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Core.Services
{
    public class InterestRuleService
    {
        private readonly IInterestRuleRepository _interestRuleRepository;

        public InterestRuleService(IInterestRuleRepository interestRuleRepository)
        {
            _interestRuleRepository = interestRuleRepository;
        }

        public void AddOrUpdateInterestRule(DateTime date, string ruleId, decimal rate)
        {
            if (rate <= 0 || rate >= 100)
                throw new ArgumentException("Interest rate must be greater than 0 and less than 100.");

            var rule = new InterestRule(date, ruleId, rate);
            _interestRuleRepository.AddOrUpdateRule(rule);
        }

        public List<InterestRule> GetAllInterestRules()
        {
            return _interestRuleRepository.GetAllRules()
                                          .OrderBy(r => r.Date)
                                          .ToList();
        }
    }
}
