using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Abstractions;

public interface ISkipPaymentService
{
    string GetPayCyclePeriod(string type, int skipCount);
    
    Task<string> HtmlPayablesList(Loan loan, DateOnly origDueDate, List<Statement> statements);
    
    Task<Period> GetPeriod(Loan loan);
}