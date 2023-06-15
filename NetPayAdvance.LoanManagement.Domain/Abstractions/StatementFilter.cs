using System;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public class StatementFilter
{
    public int? LoanId { get; set; }
    public bool? Balance { get; set; }
    public DateOnly? EndDate { get; set; }
}