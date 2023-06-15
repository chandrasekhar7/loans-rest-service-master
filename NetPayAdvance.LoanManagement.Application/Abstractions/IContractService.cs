using System;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Models.BillingStatements;
using NetPayAdvance.LoanManagement.Application.Models.Contracts;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Application.Abstractions
{
    public interface IContractService
    {
        Task<Contract> GetContract(Loan loan, DateOnly origDueDate, string payCycle, CancellationToken t = default);

        Task<Contract> SignContract(Loan loan, Contract contract, string signature, CancellationToken t = default);
        
        Task<string> CreateBillingStatement(BillingStatementRequest request, CancellationToken t = default);
    }
}
