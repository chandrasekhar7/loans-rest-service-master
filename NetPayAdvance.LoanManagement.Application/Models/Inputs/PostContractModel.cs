using NetPayAdvance.LoanManagement.Application.Models.Contracts;

namespace NetPayAdvance.LoanManagement.Application.Models.Inputs
{
    public class PostContractModel
    {
        public Contract Contract { get; set; }
                
        public string Signature { get; set; }
    }
}
