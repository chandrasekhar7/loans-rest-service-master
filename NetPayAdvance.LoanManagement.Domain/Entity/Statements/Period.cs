using System;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Statements
{
    public class Period
    {
        public DateOnly StartDate { get; }
        public DateOnly EndDate { get;}

        private Period() { }

        public Period(DateOnly start, DateOnly end)
        {
            if(start == DateOnly.MinValue || end == DateOnly.MinValue)
            {
                throw new Exception("Given date cannot be Minimum value of DateTime");
            }
            if (start >= end)
            {
                throw new Exception($"Start date {start:d} should be less than end date {end:d}");
            }
            StartDate = start;
            EndDate = end;
        }

        public bool Overlaps(Period p) => (StartDate > p.StartDate && EndDate < p.StartDate) ||
                                          (StartDate > p.EndDate && EndDate < p.EndDate);
    }
}
