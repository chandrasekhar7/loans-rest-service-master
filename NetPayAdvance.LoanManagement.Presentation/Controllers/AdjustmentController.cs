using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NetPayAdvance.LoanManagement.Application.Commands.Adjustments;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Application.Queries;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Infrastructure.Authentication;

namespace NetPayAdvance.LoanManagement.Presentation.Controllers
{
    [ApiController,ApiVersion("1.0")]
    [Route("adjustments")]
    public class AdjustmentsController : ControllerBase
    {
        private readonly IMediator mediator;

        public AdjustmentsController(IMediator med)
        {
            mediator = med ?? throw new ArgumentNullException(nameof(med));
        }
        
        /// <summary>
        /// Gets all loan adjustments with loanID
        /// </summary>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(IEnumerable<AdjustmentModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<AdjustmentModel>> GetAdjustments([FromQuery] int loanID)
        {
            return await mediator.Send(new GetAdjustmentsQuery(loanID));
        }
        
        /// <summary>
        /// Gets all loan adjustments codes
        /// </summary>
        [HttpGet, Route("codes")]
        [ProducesResponseType(typeof(IEnumerable<AdjustmentCode>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<AdjustmentCode>> GetAdjustmentCodes()
        {
            return await mediator.Send(new GetAdjustmentCodesQuery());
        }
        
        /// <summary>
        /// Gets a loan adjustment with loanID
        /// </summary>
        [HttpGet, Route("{adjustmentId:int}")]
        [ProducesResponseType(typeof(AdjustmentModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<AdjustmentModel> GetAdjustment([FromRoute] int adjustmentId) => mediator.Send(new GetAdjustmentQuery(adjustmentId));

        /// <summary>
        /// Creates manual statement adjustments from tellers.
        /// </summary>
        [HttpPost, Route("{stmtId:StatementId}")]
        [ProducesResponseType(typeof(AdjustmentModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdjustmentModel>> CreateAdjustment([FromRoute] string stmtId, [FromBody] CreateAdjustmentModel adj)
        {
            return await mediator.Send(new CreateAdjustmentCommand(stmtId, adj));
        }

        /// <summary>
        /// Creates manual adjustments for loan without statements from tellers.
        /// </summary>
        [HttpPost, Route("loan/{loanId:int}")]
        [ProducesResponseType(typeof(AdjustmentModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdjustmentModel>> CreateLoanAdjustment([FromRoute] int loanId, [FromBody] CreateAdjustmentModel adj)
        {
            return await mediator.Send(new CreateLoanAdjustmentCommand(loanId, adj));
        }

        /// <summary>
        /// Making CC and Outside debit payments for Installment and LOC loans
        /// </summary>
        [HttpPost, Route("apply-transaction"), Authorize(Policy = Policies.Management)]
        [ProducesResponseType(typeof(AdjustmentModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AdjustmentModel>> ApplyTransaction([FromBody] TransactionAdjustmentModel request)
        {
            var adj = await mediator.Send(new CreateTransactionAdjustmentCommand(request));
            return CreatedAtAction(nameof(GetAdjustment), new {adjustmentId = adj.AdjustmentId}, adj);
        }
    }
}
