using System;
using System.Runtime.Serialization;

namespace NetPayAdvance.LoanManagement.Application.ErrorHandling
{
    public class ConflictException : Exception
    {
            public ConflictException() { }

            public ConflictException(string message) : base(message) { }

            public ConflictException(string message, Exception exception) : base(message, exception) { }

            public ConflictException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}