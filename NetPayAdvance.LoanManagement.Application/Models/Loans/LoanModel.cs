
using System;
using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Application.Models.Loans
{
    public class LoanModel : IViewModel
    {
        public int LoanID { get; set; }

        public int TransID { get; set; }

        public int PowerID { get; set; }

        public int Location { get; set; }

        public DateTime CreatedOn { get; set; }
    
        public DateTime? StartedOn { get; set; }
        
        public DateTime? CompletedOn { get; set; }
        
        public decimal CreditLimit { get; set; }

        public decimal AvailableLimit { get; set; }

        public decimal APR { get; set; }
        
        public decimal CabAPR { get; set; }
        
        public int ConfigID { get; set; }
        
        public bool AutoACH { get; set; }
        
        public bool AutoDebit { get; set; }
        
        public bool CanRescind { get; set; }
        
        public LoanType LoanType { get; set; }
        
        public List<ProjectedPaymentModel> ProjectedPayments { get; set; }
        
        public LoanStatus LoanStatus { get; set; }

        public Amount Balance { get; set; }

        public LoanModel()
        {
        }

        public LoanModel(Loan loan)
        {
            LoanID = loan.LoanId;
            TransID = loan.TransID;
            PowerID = loan.PowerID;
            Location = loan.Config.Location;
            CreatedOn = loan.CreatedOn;
            LoanStatus = loan.Status;
            
            StartedOn = loan.StartedOn;
            CompletedOn = loan.ClosedOn;
            CreditLimit = loan.Config.CreditLimit;
            AvailableLimit = loan.Config.LoanType == LoanType.LineOfCredit ?
                (loan.Config.CreditLimit - loan.Balance.Amount.Principal < 0 ? 0:
                loan.Config.CreditLimit - loan.Balance.Amount.Principal) : 0;
            
            APR = loan.Config.Apr;
            CabAPR = loan.Config.CabApr;
            ConfigID = loan.ConfigID;
            LoanType = loan.Config.LoanType;
            Balance = loan.Balance.Amount;

            AutoDebit = loan.Permissions.AutoDebit;
            AutoACH = loan.Permissions.AutoACH;

            CanRescind = loan.Status == LoanStatus.Open &&
                         (DateTime.Now.Date - loan.Contracts.DateSigned).TotalDays <= 3;  

            ProjectedPayments = loan.ProjectedPayments.Select(e => new ProjectedPaymentModel(e)).ToList();
        }
    }
}
