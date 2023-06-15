using System.Collections.Generic;

namespace NetPayAdvance.LoanManagement.Application.Models.Contracts
{
    public class Contract : BaseContract<Section>
    {
        public int ContractType { get; set; }
    }

    public class BaseContract<T> where T: Section
    {
        public string CSS { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public string PageBreak { get; set; }

        public bool IsSigned { get; set; }

        public string Signature { get; set; }

        protected bool Loaded { get; set; }

        public List<T> Sections { get; set; }
    }
    
}
