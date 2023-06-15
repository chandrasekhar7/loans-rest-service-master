using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Presentation.Parameters
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>150000-20220328</example>
    public class StatementIdParameter
    {
        [Required, FromRoute, RegularExpression(@"^\d\-+\d{8}$")]
        public StatementId StatementId { get; set; }
    
        [FromBody, Required]
        public UpdateStatementModel Update { get; set; }

        public StatementIdParameter()
        {
            StatementId = new StatementId("");
            Update = new UpdateStatementModel();
        }
    }
}

