using AwesomeGICBank.Application.DTOs;
using AwesomeGICBank.Entities;

namespace AwesomeGICBank.Application.Interfaces
{
    public interface IInterestRuleService
    {
        void AddOrUpdateInterestRule(InterestRuleInputDto inputDto);
        List<InterestRule> GetAllInterestRules();
    }
}
