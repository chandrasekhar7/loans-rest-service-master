using System;
using NetPayAdvance.LoanManagement.Domain.Entity.Transactions;

namespace NetPayAdvance.LoanManagement.Application.Models.Loans;



public class TransactionAdjustmentModel
{
    public int LoanId { get; set; }

    /// <summary>
    /// The statement Id which is the OrigDueDate expressed in yyyy-MM-dd format
    /// </summary>
    /// <example>2022-03-28</example>
    public DateOnly? StatementId { get; set; }
    
    /// <summary>
    /// Amount of the transaction. Use positive numbers only
    /// </summary>
    /// <example>200</example>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// The type of transaction this is.
    /// </summary>
    /// <example>Debit</example>
    public TransactionType TransactionType { get; set; }
    
    /// <summary>
    /// A reference to the transaction id
    /// </summary>
    /// <example>1001</example>
    public int TransactionId { get; set; }
    
    /// <summary>
    /// Teller who initiated the transaction
    /// </summary>
    /// <example>FLX</example>
    public string Teller { get; set; }
    
    /// <summary>
    /// Whether the transaction attempt failed or succeeded
    /// </summary>
    /// <example>Success</example>
    public TransactionResult Result { get; set; }

    /// <summary>
    /// The payment ID of the transaction to rescind
    /// </summary>
    /// <example></example>
    public int? RescindPaymentId { get; init; }
}