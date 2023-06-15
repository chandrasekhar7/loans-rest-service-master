using System;
using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Domain.Events;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Loans;

public class Loan : DomainEntity
{
    private LoanStatus? status;
    
    public int LoanId { get; }
   
    public int ConfigID { get; }
    
    public DateTime CreatedOn { get; }

    public List<ProjectedPayment> ProjectedPayments { get; private set; } = new();
    
    public int TransID { get; }
    
    public int PowerID { get; }
    
    public DateTime? StartedOn { get;}
    
    public DateTime? ClosedOn { get; private set;}
    
    public DateTime? AccountingClosedOn { get; private set;}
    
    public LoanConfig Config { get; }
    
    public LoanContracts Contracts { get; set; }
    
    public LoanPermissions Permissions { get; set; }
    
    public LoanStatus Status
    {
        get => status ?? GetEfWorkaroundForStatus();
        set
        {
            status = value;
            if (status == LoanStatus.Cancelled)
            {
                IsCancelled = true;
            }
            else if (status == LoanStatus.Rescinded)
            {
                IsRescinded = true;
            }
            else
            {
                IsCancelled = IsRescinded = false;
            }
            
        }
    }

    public bool IsCancelled { get; private set; }
    
    public bool IsRescinded { get; private set; }
    
    public LoanBalance Balance { get; }
    
    public LoanInfo? LoanInfo { get; set; }

    private Loan() { }

    private LoanStatus GetEfWorkaroundForStatus()
    {
        if (IsRescinded)
        {
            return LoanStatus.Rescinded;
        }

        if (IsCancelled)
        {
            return LoanStatus.Cancelled;
        }

        if (ClosedOn.HasValue)
        {
            return LoanStatus.Closed;
        }

        if (StartedOn.HasValue)
        {
            return LoanStatus.Open;
        }

        return LoanStatus.NotStarted;
    }

    public Loan(int loanId, DateTime? startedOn,DateTime? closedOn, DateTime? accountingClosedOn, LoanConfig config, LoanStatus status, Amount balance)
    {
        if (accountingClosedOn.HasValue && !closedOn.HasValue)
        {
            throw new DomainLayerException("If closed for accounting must have a closed on date");
        }
        LoanId = loanId;
        StartedOn = startedOn;
        ClosedOn = closedOn;
        AccountingClosedOn = accountingClosedOn;
        Config = config;
        Status = status;
        Balance = new LoanBalance(loanId, balance);
    }

    public void AddAmount(Amount amount)
    {
        Balance.Amount += amount;
        if (Balance.Amount.Total == 0 && Config.LoanType != LoanType.LineOfCredit)
        {
            AddDomainEvent(new LoanBalanceZeroEvent(LoanId));
        }
    }

    public void CloseLoan()
    {
        if (AccountingClosedOn.HasValue || ClosedOn.HasValue || Status == LoanStatus.Closed)
        {
            throw new InvalidOperationException("Loan has already been closed");
        }
        if (Balance.Amount.Total != 0)
        {
            throw new InvalidOperationException("Loan has a balance");
        }
        if (LoanInfo == null)
        {
            throw new DomainLayerException("Pending ach is unknown");
        }
        if (!LoanInfo.PendingAch)
        {
            AccountingClosedOn = DateTime.Now;
        }
        ClosedOn = DateTime.Now;
        Status = LoanStatus.Closed;
    }

    public void Cancel()
    {
        if (Status == LoanStatus.Cancelled)
        {
            throw new InvalidOperationException("Loan is already cancelled");
        }
        if (Status != LoanStatus.NotStarted)
        {
            throw new InvalidOperationException("Cannot cancel loan");
        }

        Status = LoanStatus.Cancelled;
    }

    public void Rescind()
    {
        if (Status == LoanStatus.Rescinded)
        {
            throw new InvalidOperationException("Loan is already rescinded");
        }
        if (Status != LoanStatus.Open)
        {
            throw new InvalidOperationException("Cannot rescind loan");
        }
        Status = LoanStatus.Rescinded;
        AccountingClosedOn = ClosedOn = DateTime.Now;
    }

     public void SkipPayment(DateOnly origDate,Period period,Amount amount)
     {
         if (Config.LoanType != LoanType.InterestBearing)
         {
             throw new InvalidOperationException("Can skip payments only for installment");
         }
         var actualStatement = ProjectedPayments.First(x => x.OrigDueDate == origDate);
         actualStatement.Skipped = true;
         ProjectedPayments.Add(new ProjectedPayment(LoanId, new Amount(amount.Principal,amount.Interest,
             amount.CabFees,0,0),amount.Total,amount.Principal, period, period.EndDate));
     }
}