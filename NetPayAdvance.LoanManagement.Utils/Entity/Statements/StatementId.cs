using System;

namespace NetPayAdvance.LoanManagement.Application.Constraints;

public record StatementId
{
    public int LoanId { get; set; }
    public DateOnly OrigDueDate { get; set; }

    public StatementId(string value)
    {
        var stmtValues = value.Split('-');

        if (stmtValues.Length == 2 && int.TryParse(stmtValues[0], out var l) &&
            DateOnly.TryParseExact(stmtValues[1], "yyyyMMdd", out var d))
        {
            LoanId = l;
            OrigDueDate = d;
        }
    }

    public StatementId(int loanId, DateOnly date)
    {
        LoanId = loanId;
        OrigDueDate = date;
    }

    public static implicit operator StatementId(string s) => new StatementId(s);

    public override string ToString() => $"{LoanId.ToString()}-{OrigDueDate.ToString("yyyyMMdd")}";
}