using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions
{
    public interface IChargebackService
    {
        Task CustomerChargeback(int PaymentId, string Teller);
    }
}
