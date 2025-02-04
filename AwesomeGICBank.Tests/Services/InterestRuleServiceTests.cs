using Moq;
using NUnit.Framework;
using AwesomeGICBank.Entities;
using System;
using System.Collections.Generic;
using AwesomeGICBank.Application;
using AwesomeGICBank.Infrastructure;

namespace AwesomeGICBank.Tests.Services
{
    public class InterestRuleServiceTests
    {
        private InterestRuleService _interestRuleService;
        private Mock<IInterestRuleRepository> _mockInterestRuleRepository;

        [SetUp]
        public void Setup()
        {
            // Mock the IInterestRuleRepository
            _mockInterestRuleRepository = new Mock<IInterestRuleRepository>();

            // Initialize the InterestRuleService with the mocked repository
            _interestRuleService = new InterestRuleService(_mockInterestRuleRepository.Object);
        }

        [Test]
        public void AddOrUpdateInterestRule_ValidRule_AddsOrUpdatesRule()
        {
            var date = DateTime.ParseExact("20230615", "yyyyMMdd", null);
            var ruleId = "RULE03";
            var rate = 2.20m;

            // Act: Call the method under test
            _interestRuleService.AddOrUpdateInterestRule(date, ruleId, rate);

            // Assert that AddOrUpdateRule was called once with the correct rule
            _mockInterestRuleRepository
                .Verify(r => r.AddOrUpdateRule(It.Is<InterestRule>(ir => ir.RuleId == ruleId && ir.Rate == rate)), Times.Once);
        }

        [Test]
        public void AddOrUpdateInterestRule_InvalidRate_ThrowsException()
        {
            var date = DateTime.ParseExact("20230615", "yyyyMMdd", null);
            var ruleId = "RULE03";
            var rate = 101.00m; // Invalid rate

            // Act and Assert: Ensure an exception is thrown for invalid rate
            Assert.Throws<ArgumentException>(() =>
                _interestRuleService.AddOrUpdateInterestRule(date, ruleId, rate));
        }
    }
}
