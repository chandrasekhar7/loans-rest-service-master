using System;
using System.Runtime.Serialization;

namespace NetPayAdvance.LoanManagement.Domain.Entity;

public class DomainLayerException : Exception
{
    private DomainLayerException() { }

    public DomainLayerException(string message) : base(message) { }
}
