using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NetPayAdvance.LoanManagement.Presentation.Converters.Types;

namespace NetPayAdvance.LoanManagement.Presentation.Extensions;

public static class MvcOptionsExtensions
{
    /// <summary>
    /// Adds <see cref="TypeConverter"/> to <see cref="DateOnly"/> and <see cref="TimeOnly"/> type definitions.
    /// </summary>
    /// <param name="options">Not currently used.</param>
    public static MvcOptions UseDateOnlyTimeOnlyStringConverters(this MvcOptions options)
    {
        TypeDescriptor.AddAttributes(typeof(DateOnly), new TypeConverterAttribute(typeof(DateOnlyTypeConverter)));
        TypeDescriptor.AddAttributes(typeof(TimeOnly), new TypeConverterAttribute(typeof(TimeOnlyTypeConverter)));
        return options;
    }
}