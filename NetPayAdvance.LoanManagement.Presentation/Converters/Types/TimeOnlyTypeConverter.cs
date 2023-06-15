using System;

namespace NetPayAdvance.LoanManagement.Presentation.Converters.Types;

public class TimeOnlyTypeConverter : StringTypeConverterBase<TimeOnly>
{
    protected override TimeOnly Parse(string s) => TimeOnly.Parse(s);

    protected override string ToIsoString(TimeOnly source) => source.ToString("O");
}