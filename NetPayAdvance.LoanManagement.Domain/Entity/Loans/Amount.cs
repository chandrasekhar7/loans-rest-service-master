using System;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Loans;

public record Amount
{
    public static Amount Zero = new Amount(0, 0, 0, 0, 0);
    public decimal Total => Principal + Interest + CabFees + Nsf + LateFees;

    public decimal Principal { get; }
    public decimal Interest { get; }
    public decimal CabFees { get; }
    public decimal Nsf { get; }
    public decimal LateFees { get; }

    public Amount() { }

    public Amount(Amount amount)
    {
        Principal = amount.Principal;
        Interest = amount.Interest;
        CabFees = amount.CabFees;
        Nsf = amount.Nsf;
        LateFees = amount.LateFees;
    }

    public Amount(decimal principal, decimal interest, decimal cabFees, decimal nsf, decimal lateFees)
    {
        Principal = principal;
        Interest = interest;
        CabFees = cabFees;
        Nsf = nsf;
        LateFees = lateFees;
    }

    public static Amount operator +(Amount a, Amount b) => new Amount(a.Principal + b.Principal,
        a.Interest + b.Interest,
        a.CabFees + b.CabFees, a.Nsf + b.Nsf,
        a.LateFees + b.LateFees);

    public static Amount operator -(Amount a, Amount b) => new Amount(a.Principal - b.Principal,
        a.Interest - b.Interest,
        a.CabFees - b.CabFees, a.Nsf - b.Nsf,
        a.LateFees - b.LateFees);
    
    public static Amount operator -(Amount a) => new Amount(-a.Principal, -a.Interest, -a.CabFees, -a.Nsf, -a.LateFees);

    public Amount AddPrincipal(decimal amount) => new Amount(Principal + amount, Interest, CabFees, Nsf, LateFees);
    public Amount AddInterest(decimal amount) => new Amount(Principal, Interest + amount, CabFees, Nsf, LateFees);
    public Amount AddCabFees(decimal amount) => new Amount(Principal, Interest, CabFees + amount, Nsf, LateFees);
    public Amount AddNsf(decimal amount) => new Amount(Principal, Interest, CabFees, Nsf + amount, LateFees);
    public Amount AddLateFees(decimal amount) => new Amount(Principal, Interest, CabFees, Nsf, LateFees + amount);

    public virtual bool Equals(Amount? other)
    {
        if (other == null)
        {
            return false;
        }

        return other.Principal == Principal
               && other.Interest == Interest
               && other.CabFees == CabFees
               && other.Nsf == Nsf
               && other.LateFees == LateFees
               && other.Total == Total;
    }
};