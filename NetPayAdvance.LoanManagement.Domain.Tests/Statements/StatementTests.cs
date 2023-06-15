using System;
using NetPayAdvance.LoanManagement.Domain.Entity;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;

namespace NetPayAdvance.LoanManagement.Domain.Tests.Statements;

public class StatementTests
{
    private static Amount amount = Amount.Zero.AddPrincipal(50);

    private Statement statement = new(new StatementId(1, new DateOnly(2021, 9, 30)), amount, amount,
        new Period(new DateOnly(2021, 9, 1),
            new DateOnly(2021, 9, 30)), new DateOnly(2021, 9, 30));

    [Fact]
    public void ValidStatement() => Assert.NotNull(statement);

    [Theory]
    [InlineData("2021-08-08", 5)]
    [InlineData("2021-09-08", 5)]
    public void ExtendDueDateExceptions(string extendDate, int extension)
    {
        Assert.Throws<DomainLayerException>(() =>
            statement.ExtendDueDate(DateOnly.FromDateTime(DateTime.Parse(extendDate)), extension));
    }

    [Fact]
    public void CantExtendIfMaxExtension()
    {
        Assert.Throws<DomainLayerException>(() => statement.ExtendDueDate(new DateOnly(2021, 10, 22), 5));
    }

    [Fact]
    public void CantExtendIfNoBalance()
    {
        statement.ApplyAdjustment(Amount.Zero.AddPrincipal(-50));
        Assert.Throws<DomainLayerException>(() => statement.ExtendDueDate(new DateOnly(2021, 10, 2), 5));
    }

    [Fact]
    public void ValidExtensionDueDate()
    {
        statement.ExtendDueDate(new DateOnly(2021, 10, 2), 5);
        Assert.Equal(new DateOnly(2021, 10, 2), statement.DueDate);
    }

    [Fact]
    public void VerifyStatementAdjusts()
    {
        statement.ApplyAdjustment(Amount.Zero.AddInterest(10).AddCabFees(9).AddNsf(8).AddLateFees(7));
        Assert.Equal(new Amount(50, 10, 9, 8, 7), statement.Balance.Amount);
    }
}