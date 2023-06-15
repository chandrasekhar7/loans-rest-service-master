using System;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;

namespace NetPayAdvance.LoanManagement.Domain.Tests.Statements
{
    public class PeriodTests
    {
        [Fact]
        public void InvalidStartPeriod()
        {
            Assert.Throws<Exception>(() => new Period(DateOnly.MinValue, DateOnly.MinValue));
        }

        [Fact]
        public void InvalidDuePeriodThrows()
        {
            Assert.Throws<Exception>(() => new Period(DateOnly.FromDateTime(DateTime.Now.AddDays(-15)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(-16))));
        }

        [Theory]
        [InlineData("0001-1-1", "2021-10-5", "2021-6-5")]
        [InlineData("2021-8-5", "0001-1-1", "0001-1-1")]
        [InlineData("2021-9-5", "2021-8-5", "0001-1-1")]
        public void InvalidPeriodInput(string start, string end, string due)
        {
            Assert.ThrowsAny<Exception>(() =>
                new Period(DateOnly.FromDateTime(DateTime.Parse(start)), DateOnly.FromDateTime(DateTime.Parse(end))));
        }

        [Fact]
        public void ValidDueDate()
        {
            var period = new Period(DateOnly.FromDateTime(DateTime.Now.AddDays(-15)), DateOnly.FromDateTime(DateTime.Now.AddDays(15)));
            Assert.Equal(DateOnly.FromDateTime(DateTime.Now.AddDays(-15)), period.StartDate);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Now.AddDays(15)), period.EndDate);
        }
    }
}