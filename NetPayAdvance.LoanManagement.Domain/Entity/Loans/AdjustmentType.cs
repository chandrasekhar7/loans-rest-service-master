namespace NetPayAdvance.LoanManagement.Domain.Entity.Loans;

public enum AdjustmentType
{
    Payment = Debit.Payment,
    WriteOff = Debit.WriteOff,
    ReturnPayment = Credit.ReturnPayment,
    Discount = Debit.Discount,
    CorrectionDecrease = Debit.CorrectionDecrease,
    CorrectionIncrease = Credit.CorrectionIncrease,
    Disbursement = Credit.Disbursement,
    SkipPayment = Debit.SkipPayment,
    Rescind = Debit.Rescind
}

public enum Credit
{
    ReturnPayment = 4,
    CorrectionIncrease = 15,
    Disbursement = 64,
}

public enum Debit
{
    Payment = 1,
    WriteOff = 3,
    Discount = 6,
    CorrectionDecrease = 14,
    Rescind = 51,
    SkipPayment = 61
}