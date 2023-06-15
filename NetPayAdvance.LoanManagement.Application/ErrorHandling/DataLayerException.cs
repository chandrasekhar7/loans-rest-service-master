using System;
using System.Runtime.Serialization;

namespace NetPayAdvance.LoanManagement.Application.ErrorHandling
{
    public class DataLayerException : Exception
    {
        public DataLayerException() { }

        public DataLayerException(string message) : base(message) { }

        public DataLayerException(string message, Exception innerException) : base(message, innerException) { }

        public DataLayerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
