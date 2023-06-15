using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetPayAdvance.LoanManagement.Application.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Commands.BillingStatements;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Application.Commands.BillingStatements;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.Contracts;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.PendingChanges;
using NetPayAdvance.LoanManagement.Application.Models.Contracts;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Presentation.Controllers
{
    [ApiController,ApiVersion("1.0")]
    [Route("statements")]
    public class StatementController : ControllerBase
    {
        private readonly IMediator mediator;

        public StatementController(IMediator med)
        {
            mediator = med ?? throw new ArgumentNullException(nameof(med));
        }
        
        /// <summary>
        /// Gets all statements by loanID or endDate
        /// </summary>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(IReadOnlyList<StatementModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IReadOnlyList<StatementModel>> GetStatements([FromQuery] int? loanID, bool? hasBalance, DateOnly? endDate)
        {
            return mediator.Send(new GetStatementsQuery(loanID, hasBalance, endDate));
        } 

        /// <summary>
        /// Gets a specific statement
        /// </summary>
        [HttpGet, Route("{stmtId:StatementId}")]
        [ProducesResponseType(typeof(StatementModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<StatementModel> GetStatementById([FromRoute] string stmtId)
        {
            return mediator.Send(new GetStatementQuery(new StatementId(stmtId)));
        }

        /// <summary>
        /// Update statement endpoint for due date, fees and nsf. StatementID Format 150000-20220328
        /// </summary>
         [HttpPost, Route("{stmtId:StatementId}")]
         [ProducesResponseType(typeof(StatementModel), StatusCodes.Status200OK)]
         [ProducesResponseType(StatusCodes.Status500InternalServerError)]
         public async Task<ActionResult<StatementModel>> UpdateStatement([FromRoute] string stmtId, [FromBody] UpdateStatementModel ext)
         {
             return await mediator.Send(new UpdateStatementCommand(new StatementId(stmtId),ext.Extension));
         }

        /// <summary>
        /// Gets skip contract for Installment loans in NPA.StatementID Format 150000-20220328
        /// </summary>
        [HttpGet, Route("{stmtId:StatementId}/skip-payment")]
        [ProducesResponseType(typeof(Contract), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Contract>> GetSkipContract(string stmtId)
        {
            return Ok(await mediator.Send(new GetContractCommand(new StatementId(stmtId))));
        }
        
        /// <summary>
        /// Signs skip contract and create a statement.StatementID Format 150000-20220328
        /// </summary>
        [HttpPost, Route("{stmtId:StatementId}/skip-payment")]
        [ProducesResponseType(typeof(Contract), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Contract> SkipStatement([FromRoute] string stmtId,[FromBody] PostContractModel request)
        {
            return await mediator.Send(new SignContractCommand(new StatementId(stmtId), request));
        }

        /// <summary>
        /// Gets billing statement with billing statement ID
        /// </summary>
        [HttpGet, Route("{stmtId:StatementId}/billing-statement")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<string> GetBillingStatement([FromRoute] string stmtId)
        {
            return await mediator.Send(new GetBillingStatementCommand(new StatementId(stmtId)));
        }

        /// <summary>
        /// Builds billing statement with billing statement ID
        /// </summary>
        [HttpPost, Route("{stmtId:StatementId}/billing-statement")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<StatusCodeResult> CreateBillingStatement([FromRoute] string stmtId)
        {
            await mediator.Send(new CreateBillingStatementCommand(new StatementId(stmtId)));
            return StatusCode(201);
        }
        
        /// <summary>
        /// Adds pending changes for skip payment
        /// </summary>
        [HttpPost, Route("{stmtId:StatementId}/authorization")]
        [ProducesResponseType(typeof(Contract), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Authorization> CreateAuthorization([FromRoute] string stmtId)
        {
            return await mediator.Send(new AddPendingChangesCommand(new StatementId(stmtId)));
        }
        
        /// <summary>
        /// Cancels authorization for skip payment.StatementID Format 150000-20220328
        /// </summary>
        [HttpDelete, Route("{stmtId:StatementId}/authorization")]
        [ProducesResponseType(typeof(Contract), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Authorization> CancelAuthorization([FromRoute] string stmtId)
        {
            return await mediator.Send(new CancelPendingChangesCommand(new StatementId(stmtId)));
        }
    }
}