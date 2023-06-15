using System;
using Xunit;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Domain.Tests.Statements;

public class PendingChangesTests
{
    [Fact]
    public void CreateAuth()
    {
        var auth = new Authorization();
        auth.Create(new StatementId(1,new DateOnly(2022,2,1)),PendingChangeType.DueDate,"ILM");
        Assert.NotNull(auth);
    }
    
    [Fact]
    public void UpdateAuth()
    {
        var auth = new Authorization();
        auth.Update("ILM");
        Assert.NotNull(auth);
    }
}