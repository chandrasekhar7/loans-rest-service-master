using System;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Services
{
    public class ChargebackService : IChargebackService
    {
        private readonly IDbFacade facade;

        public ChargebackService(IDbFacade fa)
        {
            facade = fa ?? throw new ArgumentNullException(nameof(fa));
        }

        public async Task CustomerChargeback(int PaymentId, string Teller)
        {
            var values = new
            {
                PaymentId = PaymentId,
                Teller = Teller
            };
            await facade.QueryProcAsync<Task>("loan.CustomerChargeback", values);
           
        }
    }
}
