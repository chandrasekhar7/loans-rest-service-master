using System;
using System.Globalization;
using System.Linq;
using FluentValidation;
using NetPayAdvance.LoanManagement.Application.Queries;

namespace NetPayAdvance.LoanManagement.Application.Validators;

public class DateOnlyFormatValidator : AbstractValidator<GetStatementsQuery>
{
    public DateOnlyFormatValidator()
    {
        RuleFor(e => e.LoanId).Must(e => e == null || e > 0);
    }
}