using AwesomeGICBank.Entities;

namespace AwesomeGICBank.Infrastructure
{
    public interface IInterestRuleRepository
    {
        void AddOrUpdateRule(InterestRule rule);
        List<InterestRule> GetAllRules();
    }
}
