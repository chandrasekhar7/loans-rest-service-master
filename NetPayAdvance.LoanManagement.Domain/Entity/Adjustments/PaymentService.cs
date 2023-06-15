using System;
using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;

public enum Bucket
{
    Principal,
    Interest,
    CabFees,
    Nsf,
    LateFees
}
public class PaymentService
{
    private readonly List<Bucket> standardOrder = new()
    {
        Bucket.LateFees,
        Bucket.Nsf,
        Bucket.CabFees,
        Bucket.Interest,
        Bucket.Principal
    };
    
    private readonly List<Bucket> overPaymentOrder = new()
    {
        Bucket.LateFees,
        Bucket.Nsf,
        Bucket.Principal,
        Bucket.CabFees,
        Bucket.Interest,
    };

    public LoanAdjustment Rescind(int loanId, Amount balance, List<Statement> statements, Amount amount, string teller, int paymentId, LoanType loanType)
    {
        if (amount.Total < 0)
        {
            throw new DomainLayerException("You must use positive values for 'amount'");
        }

        var builder = new AdjustmentBuilder(AdjustmentType.Rescind, loanId, teller, paymentId);

        // Apply for all the statements in an installment
        if (loanType == LoanType.InterestBearing)
        {
            var remaining = -amount.Total;

            foreach (var s in statements.Where(x => x.Balance.Amount.Total > 0))
            {
                builder.ApplyStatementAdjustment(s, ApplyAmount(s.Balance.Amount, standardOrder, remaining, out remaining));
                if (remaining == 0)
                {
                    break;
                }
            }
            if (remaining < 0)
            {
                builder.ApplyAdjustment(ApplyAmount(balance - builder.Amount, overPaymentOrder, remaining, out remaining));
            }
        }
        else 
        {
            builder.ApplyAdjustment(-amount);
        }

        var la = builder.Build();
        return la;
    }

    public LoanAdjustment Payment(int loanId, Amount balance, List<Statement> statements, decimal amount, string teller, int paymentId)
    {
        if (amount < 0)
        {
            throw new DomainLayerException("You must use positive values for 'amount'");
        }

        var remaining = -amount;
        var builder = new AdjustmentBuilder(AdjustmentType.Payment, loanId, teller, paymentId);
      
        // Apply past and current due statements
        foreach (var s in statements.Where(x => x.Period.StartDate <= DateOnly.FromDateTime(DateTime.Now)))
        {
            builder.ApplyStatementAdjustment(s, ApplyAmount(s.Balance.Amount,standardOrder, remaining, out remaining));
            if (remaining == 0)
            {
                break;
            }
        }
        // Apply statements at end for overpayment
        foreach (var s in statements.Where(x => x.Period.StartDate > DateOnly.FromDateTime(DateTime.Now)).OrderByDescending(x => x.Period.StartDate))
        {
            if (remaining != 0)
            {
                builder.ApplyStatementAdjustment(s, ApplyAmount(s.Balance.Amount,standardOrder, remaining, out remaining));
            }
        }
        if (remaining < 0)
        {
            builder.ApplyAdjustment(ApplyAmount(balance + builder.Amount, overPaymentOrder, remaining, out remaining));
        }

        // If we still have some remaining we need to make the balance go negative
        if (remaining < 0)
        {
            builder.ApplyAdjustment(new Amount(remaining, 0, 0, 0, 0));
        }
        var la = builder.Build();
        return la;
    }
    

    private Amount ApplyAmount(Amount balance,List<Bucket> buckets, decimal amount, out decimal remaining)
    {
        if (amount > 0)
        {
            throw new InvalidOperationException($"{nameof(amount)} cannot be greater than 0");
        }

        Amount adjustment = Amount.Zero;
        foreach (var bucket in buckets)
        {
            adjustment = bucket switch
            {
                Bucket.Principal => AddPrincipal(balance, adjustment, amount, out amount),
                Bucket.Interest => AddInterest(balance, adjustment, amount, out amount),
                Bucket.CabFees => AddCabFees(balance, adjustment, amount, out amount),
                Bucket.Nsf => AddNsf(balance, adjustment, amount, out amount),
                Bucket.LateFees => AddLateFees(balance, adjustment, amount, out amount),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        remaining = amount;
        return adjustment;
    }

    private Amount AddPrincipal(Amount balance, Amount adjustment, decimal amount, out decimal a)
    {
        a = amount;
        if (balance.Principal > 0)
        {
            var adj = -amount > balance.Principal ? -balance.Principal : amount;
            a -= adj;
            return adjustment.AddPrincipal(adj);
        }

        return adjustment;
    }

    private Amount AddInterest(Amount balance, Amount adjustment, decimal amount, out decimal a)
    {
        a = amount;
        if (balance.Interest > 0)
        {
            var adj = -amount > balance.Interest ? -balance.Interest : amount;
            a -= adj;
            return adjustment.AddInterest(adj);
        }

        return adjustment;
    }

    private Amount AddCabFees(Amount balance, Amount adjustment, decimal amount, out decimal a)
    {
        a = amount;
        if (balance.CabFees > 0)
        {
            var adj = -amount > balance.CabFees ? -balance.CabFees : amount;
            a -= adj;
            return adjustment.AddCabFees(adj);
        }

        return adjustment;
    }

    private Amount AddNsf(Amount balance, Amount adjustment, decimal amount, out decimal a)
    {
        a = amount;
        if (balance.Nsf > 0)
        {
            var adj = -amount > balance.Nsf ? -balance.Nsf : amount;
            a -= adj;
            return adjustment.AddNsf(adj);
        }

        return adjustment;
    }

    private Amount AddLateFees(Amount balance, Amount adjustment, decimal amount, out decimal a)
    {
        a = amount;
        if (balance.LateFees > 0)
        {
            var adj = -amount > balance.LateFees ? -balance.LateFees : amount;
            a -= adj;
            return adjustment.AddLateFees(adj);
        }

        return adjustment;
    }
}