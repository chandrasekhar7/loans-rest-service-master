using System;
using System.Runtime.Serialization;

namespace NetPayAdvance.LoanManagement.Application.ErrorHandling
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : this("Not found")
        {
        }

        public NotFoundException(string? message) : base(message)
        {
        }
    }
}