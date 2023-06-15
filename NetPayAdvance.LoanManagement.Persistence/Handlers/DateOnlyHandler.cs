using System;
using System.Data;
using Dapper;

namespace NetPayAdvance.LoanManagement.Persistence.Handlers;

public class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.Value = value.ToString();
    }

    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);//DateOnly.ParseExact(value.ToString(), "yyyy-MM-dd");
}