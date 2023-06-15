using System;
using System.Runtime.Serialization;

namespace NetPayAdvance.LoanManagement.Application.ErrorHandling;

public class UnauthorizedException : Exception
{
    public UnauthorizedException()
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected UnauthorizedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}