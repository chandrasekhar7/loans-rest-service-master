using System;
using FluentValidation;
using NetPayAdvance.LoanManagement.Application.Models.Loans;

namespace NetPayAdvance.LoanManagement.Application.Validators;

public class TransactionAdjustmentModelValidator : AbstractValidator<TransactionAdjustmentModel>
{
    public TransactionAdjustmentModelValidator()
    {
        RuleFor(e => e.Amount).GreaterThan(0);
        RuleFor(e => e.Teller).Length(3);
        RuleFor(e => e.LoanId).GreaterThan(0);
        RuleFor(e => e.TransactionId).GreaterThan(0);
        RuleFor(e => e.TransactionType).NotNull();
        RuleFor(e => e.Result).IsInEnum();
    }
}