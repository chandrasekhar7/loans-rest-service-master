using System;
using System.Data;
using System.Threading;
using Moq;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.Contracts;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;
using Xunit;

namespace NetPayAdvance.LoanManagement.Application.Tests.Commands.Statements.Contracts;

public class GetContractCommandTests
{
    private readonly Mock<IContractService> contract = new();
    private readonly Mock<IDbFacade> facade = new();
    private readonly Mock<ILoanRepository> repo = new();
    private GetContractCommandHandler handler;

    public GetContractCommandTests()
    {
        repo.Setup(m => m.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => GetLoan());
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
    public void GetContract()
    {
        facade.Setup(m => m.QueryFirstOrDefaultAsync<string>(It.IsAny<string>(), It.IsAny<object>(), 
                   It.IsAny<IDbTransaction>()))
                  .ReturnsAsync(() => { return "S"; });
        
    }
    
}