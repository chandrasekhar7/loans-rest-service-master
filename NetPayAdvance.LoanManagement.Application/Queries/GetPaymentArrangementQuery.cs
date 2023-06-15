using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Queries
{
    public class GetPaymentArrangementQuery : IRequest<PaymentArrangement>
    {
        public int? LoanID { get; init; } = default!;

        public class GetPaymentArrangementQueryHandler : IRequestHandler<GetPaymentArrangementQuery, PaymentArrangement>
        {
            private readonly IDbFacade facade;

            public GetPaymentArrangementQueryHandler(IDbFacade fa)
            {
                facade = fa ?? throw new ArgumentNullException(nameof(fa));
            }

            public async Task<PaymentArrangement> Handle(GetPaymentArrangementQuery request, CancellationToken t = default)
            {
                return (await facade.QueryAsync<PaymentArrangement>(@"SELECT * FROM PaydayFlex.loan.PaymentArrangement WHERE LoanID = @LoanID", new { request.LoanID })).FirstOrDefault();
            }
        }
    }
}

