using Moq;
using NUnit.Framework;
using AwesomeGICBank.Entities;
using AwesomeGICBank.Application;
using AwesomeGICBank.Application.DTOs;
using AwesomeGICBank.Infrastructure;
using System;

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
            // Arrange
            var inputDto = new InterestRuleInputDto
            {
                Date = DateTime.ParseExact("20230615", "yyyyMMdd", null),
                RuleId = "RULE03",
                Rate = 2.20m
            };

            // Act: Call the method under test
            _interestRuleService.AddOrUpdateInterestRule(inputDto);

            // Assert that AddOrUpdateRule was called once with the correct rule
            _mockInterestRuleRepository
                .Verify(r => r.AddOrUpdateRule(It.Is<InterestRule>(ir =>
                    ir.Date == inputDto.Date &&
                    ir.RuleId == inputDto.RuleId &&
                    ir.Rate == inputDto.Rate)), Times.Once);
        }

        [Test]
        public void AddOrUpdateInterestRule_InvalidRate_ThrowsException()
        {
            // Arrange
            var inputDto = new InterestRuleInputDto
            {
                Date = DateTime.ParseExact("20230615", "yyyyMMdd", null),
                RuleId = "RULE03",
                Rate = 101.00m // Invalid rate
            };

            // Act and Assert: Ensure an exception is thrown for invalid rate
            Assert.Throws<ArgumentException>(() =>
                _interestRuleService.AddOrUpdateInterestRule(inputDto));
        }
    }
}