using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetPayAdvance.LoanManagement.Application.Queries;
using System;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Models.Common;

namespace NetPayAdvance.LoanManagement.Presentation.Controllers
{
    [ApiController,ApiVersion("1.0")]
    public class DataController : ControllerBase
    {
        private readonly IMediator mediator;

        public DataController(IMediator med)
        {
            mediator = med ?? throw new ArgumentNullException(nameof(med));
        }
        
        /// <summary>
        /// Gets a payment arrangement with loanID
        /// </summary>
        [HttpGet, Route("payment-arrangement")]
        [ProducesResponseType(typeof(PaymentArrangement), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<PaymentArrangement> GetPaymentArrangement([FromQuery] int loanID)
        {
            return await mediator.Send(new GetPaymentArrangementQuery() { LoanID = loanID });
        }
    }
}
