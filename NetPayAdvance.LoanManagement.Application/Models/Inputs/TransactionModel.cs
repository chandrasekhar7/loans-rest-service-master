using System;
using System.Collections.Generic;
using NetPayAdvance.LoanManagement.Domain.Entity.Transactions;

namespace NetPayAdvance.LoanManagement.Application.Models.Inputs;

public class TransactionModel
{
    public IReadOnlyList<CardTransactionViewModel> CardTransactions { get; set; }
    
    public IReadOnlyList<AchTransactionViewModel> AchTransactions { get; set; }
}

public class CardTransactionViewModel
{
    public int Id { get; init; }
    public int LoanId { get; init; }
    public TransactionType TransactionType { get; init; }
    public decimal Amount { get; set; }
    public DateTime CreatedOn { get; init; }
    public string Teller { get; init; } = default!;
    public int CardToken { get; init; }
    public DateTime StatusDate { get; init; }
    public string ReturnCode { get; init; } = default!;
    public int MerchantId { get; init; }
    public string? ReturnMessage { get; init; }
    public string? RefNum { get; init; }
    public string? LastFour { get; init; }
}

public class AchTransactionViewModel
{
    public int Id { get; init; }
    public int LoanId { get; init; }
    public TransactionType TransactionType { get; init; }
    public decimal Amount { get; init; }
    public DateTime CreatedOn { get; init; }
    public string Teller { get; init; } = default!;
    public DateTime StatusDate { get; init; }
    public string? ReturnCode { get; init; } = default!;
    public string? ReturnMessage { get; init; }
}