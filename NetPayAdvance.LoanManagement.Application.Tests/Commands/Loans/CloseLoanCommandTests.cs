using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Loans;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Application.Users;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;
using Xunit;

namespace NetPayAdvance.LoanManagement.Application.Tests.Commands.Loans;

public class CloseLoanCommandTests : BaseTestContext
{
    private readonly Mock<IHistoryNotesService> history = new();
    private readonly Mock<IUserService> user = new();
    private readonly Mock<ILoanRepository> repo = new();
    private CloseLoanCommandHandler handler;

    public CloseLoanCommandTests()
    {
        user.Setup(m => m.GetUser()).Returns(new User("NMR"));
    }

    private static Loan GetLoan()
    {
        var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(1000, 0.1m, 1m, LoanType.LineOfCredit, 701),
            LoanStatus.Open, new Amount(500, 10, 10, 0, 0));
        loan.LoanInfo = new LoanInfo(false);
        loan.Permissions = new LoanPermissions() {LoanId = 1, AutoDebit = true, AutoACH = true};
        return loan;
    }
    
    [Fact]
    public async Task ThrowsIfBalance()
    {
        var loan = GetLoan();
        repo.Setup(m => m.GetByIdWithPendingAchAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => loan);
        handler = new CloseLoanCommandHandler(repo.Object, history.Object, user.Object);
        var ex = await Assert.ThrowsAsync<ApplicationLayerException>(() => 
            handler.Handle(new CloseLoanCommand(1,"Closing LOC")));
        Assert.Equal("Loan has a balance", ex.Message);
    }

    [Fact]
    public async Task ThrowsIfLoanClosed()
    {
        var loan = GetLoan();
        loan.Status = LoanStatus.Closed;
        repo.Setup(m => m.GetByIdWithPendingAchAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => loan);
        handler = new CloseLoanCommandHandler(repo.Object, history.Object, user.Object);
        var ex = await Assert.ThrowsAsync<ApplicationLayerException>(() => 
            handler.Handle(new CloseLoanCommand(1,"Closing LOC")));
        Assert.Equal("Loan has already been closed", ex.Message);
    }
    
    [Fact]
    public async Task ThrowsForInvalidLoanInfo()
    {
        var loan = GetLoan();
        loan.AddAmount(new Amount(-500,-10,-10,0,0));
        loan.LoanInfo = null;
        repo.Setup(m => m.GetByIdWithPendingAchAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => loan);
        handler = new CloseLoanCommandHandler(repo.Object, history.Object, user.Object);
        var ex = await Assert.ThrowsAsync<ApplicationLayerException>(() => 
            handler.Handle(new CloseLoanCommand(1,"Closing LOC")));
        Assert.Equal("Pending ach is unknown", ex.Message);
    }

    [Fact]
    public async Task ClosesLOC()
    {
        var loan = GetLoan();
        loan.AddAmount(new Amount(-500,-10,-10,0,0));
        repo.Setup(m => m.GetByIdWithPendingAchAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => loan);
        handler = new CloseLoanCommandHandler(repo.Object, history.Object, user.Object);
        var c = await handler.Handle(new CloseLoanCommand(1, "Notes"), CancellationToken.None);
        history.Verify(mock => mock.InsertNotes(It.IsAny<HistoryNotes>()), Times.Once());
    }
}