using System.Linq;
using Mapster;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Mapster.Mappers
{
    public partial class StatementToStatementModelMapper : BaseMapper<Statement,StatementModel>, IDomainEntityToViewModelMapper<Statement,StatementModel>
    {
    }
    
    public partial class StatementToStatementModelMapper
    {
        protected override TypeAdapterSetter<Statement, StatementModel> Configure(TypeAdapterSetter<Statement, StatementModel> typeAdapterSetter)
        {
            return base.Configure(typeAdapterSetter)
                       .Map(dest => dest.StatementId, src => src.StatementId.ToString())
                       .Map(dest => dest.Balance, src => src.Balance)
                       .Map(dest => dest.Amount, src => src.Amount)
                       .Map(dest => dest.Period, src => src.Period)
                       .Map(dest => dest.OrigDueDate, src => src.OrigDueDate)
                       .Map(dest => dest.DueDate, src => src.DueDate);
        }
    }
}

