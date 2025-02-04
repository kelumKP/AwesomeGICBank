using AwesomeGICBank.Application.DTOs;
using AwesomeGICBank.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Application.Interfaces
{
    public interface IInterestRuleService
    {
        void AddOrUpdateInterestRule(InterestRuleInputDto inputDto);
        List<InterestRule> GetAllInterestRules();
    }
}
