using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Models.BillingStatements;
using NetPayAdvance.LoanManagement.Application.Models.Contracts;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Infrastructure.Contracts.Configuration;

namespace NetPayAdvance.LoanManagement.Infrastructure.Contracts.Services
{
    public class ContractService : IContractService
    {
        private readonly IBaseContractService contract;
        private readonly ISkipPaymentService skipService;
        private readonly IStatementRepository repo;
        private readonly IAdjustmentAggregateRepository aggRepo;

        public ContractService(IBaseContractService cs, ISkipPaymentService service, IStatementRepository stRepo, IAdjustmentAggregateRepository agg)
        {
            contract = cs ?? throw new ArgumentNullException(nameof(cs));
            skipService = service ?? throw new ArgumentNullException(nameof(service));
            repo = stRepo ?? throw new ArgumentNullException(nameof(stRepo));
            aggRepo = agg ?? throw new ArgumentNullException(nameof(agg));
        }

        public async Task<Contract> GetContract(Loan loan, DateOnly origDueDate, string payCycle, CancellationToken t = default)
        {
            try
            {
                var statements = await repo.GetAsync(new StatementFilter() { LoanId = loan.LoanId }, t);
                var aggregates = await aggRepo.GetByIdAsync(loan.LoanId, t);

                var template = await contract.GetContract<string>(17.ToString(),t);
                var sb = new StringBuilder(template);
                sb.Replace(template, LoanBuilder.Build(template, loan, statements.ToList()))

                  .Replace(TILA.DueDate, origDueDate.ToString("MM/dd/yyyy"))

                  .Replace(TILA.NextDueDate, loan.ProjectedPayments.First(x => 
                       (x.OrigDueDate != origDueDate && x.Skipped == false && x.OrigDueDate > DateOnly.FromDateTime(DateTime.Now)) ||
                       (x.OrigDueDate == origDueDate)).OrigDueDate.ToShortDateString())

                  .Replace(TILA.ExtendedTime, skipService.GetPayCyclePeriod(payCycle, 1))

                  .Replace(TILA.Payables, await skipService.HtmlPayablesList(loan, origDueDate, statements.ToList()));

                var request = new TILARequest(loan.PowerID, loan.LoanId, new Contract() { ContractType = 17 }, sb.ToString(),"");
                return await contract.BuildContract<Contract>(request, t);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Contract> SignContract(Loan loan, Contract cont, string signature, CancellationToken t = default)
        {
            try
            {
                var request = new TILARequest(loan.PowerID, loan.LoanId, cont, cont.Sections[0].Contract, signature);
                return await contract.SignContract<Contract>(request,t);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }          
        }

        public async Task<string> CreateBillingStatement(BillingStatementRequest request, CancellationToken t = default)
        {
            try
            {
                return await contract.PostBillingStatement<string>(request,t);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }          
        }
    }
}
