// AwesomeGICBank.Tests/InterestRuleServiceTests.cs
using NUnit.Framework;
using AwesomeGICBank.Core.Services;
using AwesomeGICBank.Entities;
using AwesomeGICBank.Infrastructure.Repositories;
using AwesomeGICBank.Core.Interfaces;

namespace AwesomeGICBank.Tests
{
    public class InterestRuleServiceTests
    {
        private InterestRuleService _interestRuleService;
        private IInterestRuleRepository _interestRuleRepository;

        [SetUp]
        public void Setup()
        {
            _interestRuleRepository = new InterestRuleRepository();
            _interestRuleService = new InterestRuleService(_interestRuleRepository);
        }

        [Test]
        public void AddOrUpdateInterestRule_ValidRule_AddsOrUpdatesRule()
        {
            var date = DateTime.ParseExact("20230615", "yyyyMMdd", null);
            var ruleId = "RULE03";
            var rate = 2.20m;

            _interestRuleService.AddOrUpdateInterestRule(date, ruleId, rate);

            var rules = _interestRuleService.GetAllInterestRules();
            Assert.AreEqual(1, rules.Count);
            Assert.AreEqual(ruleId, rules[0].RuleId);
            Assert.AreEqual(rate, rules[0].Rate);
        }

        [Test]
        public void AddOrUpdateInterestRule_InvalidRate_ThrowsException()
        {
            var date = DateTime.ParseExact("20230615", "yyyyMMdd", null);
            var ruleId = "RULE03";
            var rate = 101.00m; // Invalid rate

            Assert.Throws<ArgumentException>(() =>
                _interestRuleService.AddOrUpdateInterestRule(date, ruleId, rate));
        }
    }
}
