using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using NetPayAdvance.LoanManagement.Application.Commands.Loans;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Application.Queries;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NetPayAdvance.LoanManagement.Infrastructure.Authentication;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.CreateChargebackCommand;

namespace NetPayAdvance.LoanManagement.Presentation.Controllers
{
    [ApiController, ApiVersion("1.0")]
    [Route("loans")]
    public class LoanController : ControllerBase
    {
        private readonly IMediator mediator;

        public LoanController(IMediator med)
        {
            mediator = med ?? throw new ArgumentNullException(nameof(med));
        }

        /// <summary>
        /// Gets all loans of a customer with powerID
        /// </summary>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(IReadOnlyList<LoanModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IReadOnlyList<LoanModel>> GetLoans([FromQuery] int? powerID, [FromQuery] LoanStatus[]? statuses) =>
            mediator.Send(new GetLoansQuery(new LoanFilter(powerID ?? throw new ArgumentNullException(nameof(powerID)), statuses)));

        /// <summary>
        /// Gets a specific loan
        /// </summary>
        [HttpGet, Route("{loanID:int}")]
        [ProducesResponseType(typeof(LoanModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<LoanModel> GetLoan([FromRoute] int loanID) => mediator.Send(new GetLoanQuery(loanID));

        /// <summary>
        /// Gets all projected statements of a loan with loanID
        /// </summary>
        [HttpGet, Route("{loanID:int}/projected-payments")]
        [ProducesResponseType(typeof(IReadOnlyList<ProjectedPaymentModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IReadOnlyList<ProjectedPaymentModel>> GetProjectedPayments([FromRoute] int loanID)
        {
            return await mediator.Send(new GetProjectedPaymentsQuery() { LoanID = loanID });
        }

        /// <summary>
        /// Closes LOC with a history note
        /// </summary>
        [HttpPost, Route("{loanID:int}/close-loan"), Authorize(Policy = Policies.Admin)]
        [ProducesResponseType(typeof(LoanModel), StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<LoanModel> CloseLoan([FromRoute] int loanID,[FromBody] CloseLoanModel close)
        {
            return await mediator.Send(new CloseLoanCommand(loanID,close.Notes));
        }
        /// <summary>
        /// Chargeback for a particular Payment
        /// </summary>
        [HttpPost, Route("{loanID:int}/chargeback/{paymentID:int}")]
        [ProducesResponseType(typeof(LoanModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<StatusCodeResult> CreateChargeback([FromRoute] int loanID, [FromRoute] int paymentID)
        {
             await mediator.Send(new CreateChargebackCommand(loanID, paymentID));
            return StatusCode(201);
        }
    }
}
