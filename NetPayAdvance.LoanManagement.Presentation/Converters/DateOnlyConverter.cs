using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetPayAdvance.LoanManagement.Presentation.Converters;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private readonly string serializationFormat;
    

    public DateOnlyConverter() : this(null)
    {
    }

    public DateOnlyConverter(string? serializationFormat)
    {
        this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
    }

    public override DateOnly Read(ref Utf8JsonReader reader, 
        Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateOnly.ParseExact(value!, AcceptedFormats.DateFormats);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, 
        JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(serializationFormat));
}
