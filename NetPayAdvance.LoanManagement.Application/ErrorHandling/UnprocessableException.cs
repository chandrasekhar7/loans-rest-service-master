using System;
using System.Runtime.Serialization;

namespace NetPayAdvance.LoanManagement.Application.ErrorHandling;

public class UnprocessableException : Exception
{
    public UnprocessableException()
    {
    }

    public UnprocessableException(string? message) : base(message)
    {
    }

    public UnprocessableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected UnprocessableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}