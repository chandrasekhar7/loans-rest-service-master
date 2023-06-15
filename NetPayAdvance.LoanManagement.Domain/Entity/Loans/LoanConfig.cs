namespace NetPayAdvance.LoanManagement.Domain.Entity.Loans;

public record LoanConfig(decimal CreditLimit,decimal Apr, decimal CabApr, LoanType LoanType, int Location);