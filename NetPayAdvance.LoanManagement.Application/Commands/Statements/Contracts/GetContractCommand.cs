using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;
using NetPayAdvance.LoanManagement.Application.Models.Contracts;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Commands.Statements.Contracts;

public record GetContractCommand(StatementId StatementId) : IRequest<Contract>;

public class GetContractCommandHandler : IRequestHandler<GetContractCommand, Contract>
{
    private readonly IContractService contractService;
    private readonly ILoanRepository repo;
    private readonly IDbFacade facade;

    public GetContractCommandHandler(ILoanRepository loan, IContractService service, IDbFacade fa)
    {
        repo = loan ?? throw new ArgumentNullException(nameof(loan));
        contractService = service ?? throw new ArgumentNullException(nameof(service));
        facade = fa ?? throw new ArgumentNullException(nameof(fa));
    }
    
    public async Task<Contract> Handle(GetContractCommand request, CancellationToken t = default)
    {
        try
        {
            var loan = await repo.GetByIdAsync(request.StatementId.LoanId,t) ?? throw new NotFoundException();
            var payCycle = await facade.QueryFirstOrDefaultAsync<string>(@"SELECT PayCycle FROM  PaydayFlex.loan.vCustomers WHERE PowerID = @PowerID", new { loan.PowerID });
            var contract = await contractService.GetContract(loan, request.StatementId.OrigDueDate, payCycle,t) ?? throw new NotFoundException();
            return contract;
        }
        catch (Exception e)
        {
            throw new ApplicationLayerException(e.Message);
        }
    }
}

public class GetContractCommandValidator : AbstractValidator<GetContractCommand>
{
    public GetContractCommandValidator()
    {
        RuleFor(e => e.StatementId).SetValidator(new StatementIdValidator());
    }
}

