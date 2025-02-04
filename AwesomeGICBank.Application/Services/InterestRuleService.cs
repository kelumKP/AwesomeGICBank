using AwesomeGICBank.Entities;
using AwesomeGICBank.Infrastructure;
using AwesomeGICBank.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AwesomeGICBank.Application
{
    public class InterestRuleService
    {
        private readonly IInterestRuleRepository _interestRuleRepository;

        public InterestRuleService(IInterestRuleRepository interestRuleRepository)
        {
            _interestRuleRepository = interestRuleRepository;
        }

        public void AddOrUpdateInterestRule(InterestRuleInputDto inputDto)
        {
            if (inputDto.Rate <= 0 || inputDto.Rate >= 100)
                throw new ArgumentException("Interest rate must be greater than 0 and less than 100.");

            var rule = new InterestRule(inputDto.Date, inputDto.RuleId, inputDto.Rate);
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
