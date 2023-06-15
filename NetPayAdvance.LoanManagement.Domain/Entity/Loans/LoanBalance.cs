namespace NetPayAdvance.LoanManagement.Domain.Entity.Loans;

public class LoanBalance : DomainEntity
{
    public int LoanId { get; }
    public Amount Amount { get; set; }
    

    private LoanBalance()
    {
    }

    public LoanBalance(int loanId, Amount amount)
    {
        LoanId = loanId;
        Amount = amount;
    }
}