using NumericWordsConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetPayAdvance.Common;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Infrastructure.Contracts.Services
{
    public class SkipPaymentService : ISkipPaymentService
    {
        private readonly IAdjustmentAggregateRepository aggRepo; 
        private readonly IDbFacade facade;

        public SkipPaymentService(IAdjustmentAggregateRepository agg, IDbFacade fa)
        {
            facade = fa ?? throw new ArgumentNullException(nameof(fa));
            aggRepo = agg ?? throw new ArgumentNullException(nameof(agg));
        }
        
        public string GetPayCyclePeriod(string type, int skipCount)
        {
            var sb = new StringBuilder();
            var converter = new NumericWordsConverter();
            if (type.Equals(PayCycleType.Weekly.ToString()))
                sb.Append(converter.ToWords(skipCount * 1) + " week");
            else if (type.Equals(PayCycleType.BiWeekly.ToString()) || type.Equals(PayCycleType.SemiMonthly.ToString()))
                sb.Append(converter.ToWords(skipCount * 2) + " weeks");
            else
                sb.Append(converter.ToWords(skipCount * 1) + " month");
            return sb.ToString();
        }

        public async Task<Period> GetPeriod(Loan loan)
        {
            var maxDueDate = loan.ProjectedPayments.Last().Period.EndDate;
            var nextPayDate = await facade.QueryFirstOrDefaultAsync<DateOnly>(@"SELECT PayDate FROM PaydayFlex.dbo.FN_Paydates(@PowerID, @Paydates, @StartDate)",
                new { loan.PowerID, Paydates = 1, StartDate = maxDueDate.AddDays(1) });
            return new Period(maxDueDate.AddDays(1), nextPayDate);
        }

        private static bool TryGetSkipped(AdjustmentAggregate loan, DateOnly origDueDate, out LoanAdjustment? adj)
        {
            var payment = loan.Adjustments.LastOrDefault(a => a.Adjustment.AdjustmentCodeID == 61);
            adj = payment;
            return payment != null;
        }

        private async Task<AdjustmentAggregate> GetAdjustments(StatementId statementId)
        {
            return await aggRepo.GetByIdWithAdjustments(statementId); 
        }

        public async Task<string> HtmlPayablesList(Loan loan, DateOnly origDueDate, List<Statement> statements)
        {
            var sTable = "style=\"display:flex;justify-content:center\"";
            var sTD = "style=\"text-align:left;padding:4pt;font-size:10pt;\"";
            var sb = new StringBuilder();
            var i = 0;
            var lastPayable = new StringBuilder("");
            var period = await GetPeriod(loan);
            sb.Append($"<table {sTable}>");

            foreach (var projectedPayment in loan.ProjectedPayments)
            {
                sb.Append("<tr>");

                var statement = statements.FirstOrDefault(x => x.OrigDueDate == projectedPayment.OrigDueDate);

                var adjustments = await GetAdjustments(new StatementId(loan.LoanId, projectedPayment.OrigDueDate));

                var statementAdjustments = adjustments?.Adjustments.Where(x => x.StatementAdjustments.Count > 0 && 
                                                                       x.StatementAdjustments.Any(y => y.OrigDueDate == projectedPayment.OrigDueDate)).ToList();

                if (projectedPayment.Skipped && TryGetSkipped(adjustments, projectedPayment.OrigDueDate, out var skipped))
                {
                    sb.Append($"<td {sTD}> Payment {++i} Due: ").Append($"{projectedPayment?.Payment:C}</td>");
                    sb.Append($"<td {sTD}> Amt Paid: ").Append("<strong>SKIPPED BY AGREEMENT</strong></td>");
                    sb.Append($"<td {sTD}> on: {skipped!.Adjustment.CreatedOn.ToShortDateString()}</td>");
                }
                else if (projectedPayment.OrigDueDate == origDueDate)
                {
                    sb.Append($"<td {sTD}> Payment {++i} Due: ").Append($"{projectedPayment?.Payment:C}</td>");
                    sb.Append($"<td {sTD}> Amt Paid: ").Append("<strong>SKIPPED BY AGREEMENT</strong></td>");
                    sb.Append($"<td {sTD}> on: {DateTime.Now.ToShortDateString()}</td>");
                    
                    lastPayable.Append($"<td {sTD}> Payment {loan.ProjectedPayments.Count + 1} Due: ").Append($"{projectedPayment?.Payment:C}</td>");
                    lastPayable.Append($"<td {sTD}> Amt Paid: ").Append($"{0:C}</td>");
                    lastPayable.Append($"<td {sTD}> on: {period.EndDate.ToShortDateString()}</td>");
                }
                else
                {
                    sb.Append($"<td {sTD}> Payment {++i} Due: ").Append($"{projectedPayment.Payment:C}</td>");
                    
                    if (statement != null && statementAdjustments != null && statementAdjustments.Any())
                    {
                       var payments = statementAdjustments.Where(x => x.PaymentID != null).Select(x => x.Adjustment).ToList();
                        if (payments.Any())
                        {
                            sb.Append($"<td {sTD}> Amt Paid: ").Append($"{-payments.Sum(x => x.Amount.Total):C}</td>");
                            sb.Append($"<td {sTD}> on: {payments.Max(x => x.CreatedOn).ToShortDateString()}</td>");
                        }
                        else
                        {
                            sb.Append($"<td {sTD}> Amt Paid: ").Append($"{0:C}</td>");
                            sb.Append($"<td {sTD}> on: {projectedPayment.OrigDueDate.ToShortDateString()}</td>");
                        }
                    }
                    else
                    {
                        sb.Append($"<td {sTD}> Amt Paid: ").Append($"{0:C}</td>");
                        sb.Append($"<td {sTD}> on: {projectedPayment.OrigDueDate.ToShortDateString()}</td>");
                    }
                }
                sb.Append("</tr>");
            }
           
            if (i == loan.ProjectedPayments.Count)
            {
                sb.Append(lastPayable.ToString());
            }
            sb.Append("</table>");
            return sb.ToString();
        }
    }
}
