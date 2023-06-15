using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public class LoanFilter
{
    public int PowerId { get; set; }
    public LoanStatus[]? Statuses { get; set; }

    public LoanFilter(int powerId, IEnumerable<LoanStatus>? statuses = null)
    {
        PowerId = powerId;
        Statuses = statuses?.ToArray();
    }
}