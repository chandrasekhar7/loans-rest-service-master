using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPayAdvance.LoanManagement.Application.Models.Contracts;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Infrastructure.Contracts.Configuration
{
    public static class LoanBuilder
    {
        public static string Build(string contract, Loan l, List<Statement> statements)
        {
            var sb = new StringBuilder(contract);
            sb.Replace(TILA.LoanAPR, ((int)(l.Config.Apr * 100)).ToString())
                .Replace(TILA.LoanID, l.LoanId.ToString())
                .Replace(TILA.Today, DateTime.Now.ToShortDateString())
                .Replace(TILA.TransID, l.LoanId == 0 ? "New Transaction" : l.LoanId.ToString());

            if (statements.Any() && l.ProjectedPayments.Any())
            {
                sb.Replace(TILA.PDTE, statements.First().OrigDueDate.ToShortDateString())
                    .Replace(TILA.ServiceFee, l.ProjectedPayments.Sum(x => x.Amount.CabFees).ToString("C"))
                    .Replace(TILA.ServiceFeePerPayment, l.ProjectedPayments.First().Amount.CabFees.ToString("C"))
                    .Replace(TILA.Commission, l.ProjectedPayments.Sum(x => x.Amount.CabFees) + l.ProjectedPayments.Sum(x => x.Amount.Interest).ToString("C"))
                    .Replace(TILA.CAB, l.ProjectedPayments.Sum(x => x.Amount.CabFees).ToString("C"))
                    .Replace(TILA.Interest, l.ProjectedPayments.Sum(x => x.Amount.Interest).ToString("C"))
                    .Replace(TILA.AmountFinancedLessFee, l.ProjectedPayments.Sum(x => x.Payment).ToString("C"))
                    .Replace(TILA.AmountFinanced, l.ProjectedPayments.Sum(x => x.Payment).ToString("C"))
                    .Replace(TILA.SignatureDate, l.CreatedOn.ToShortDateString());
            }
            return sb.ToString();
        }
    }
}
