using System;
using NetPayAdvance.LoanManagement.Application.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Models.Loans
{
    // public class BaseAdjustmentModel : BucketModel
    // {
    //     public int LoanID { get; set; }
    //
    //     public int AdjustmentID { get; set; }
    //
    //     public int AdjustmentCodeID { get; set; }
    //
    //     public string AdjustmentType { get; set; }  
    //
    //     public DateTime CreatedOn { get; set; }
    //            
    //     public decimal Total => Principal + Interest + CabFees + (decimal)NSF + (decimal)LateFees;
    //
    //     public string Teller { get; set; }
    // }
    //
    // public class LoanAdjustmentModel : BaseAdjustmentModel,IViewModel
    // {
    //     public int? PaymentID { get; set; }
    // }
    //
    // public class StatementAdjustmentModel : BaseAdjustmentModel,IViewModel
    // {
    //     public DateTime OrigDueDate { get; set; }
    //
    //     public int? LoanAdjustmentID { get; set; }
    // }
}
