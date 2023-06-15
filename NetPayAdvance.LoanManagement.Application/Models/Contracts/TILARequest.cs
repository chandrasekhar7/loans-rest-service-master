
namespace NetPayAdvance.LoanManagement.Application.Models.Contracts
{
    public class TILARequest
    {
        public int LoanID { get; set; }

        public int PowerID { get; set; }

        public Contract? Contract { get; set; }

        public string TILA { get; set; }
        
        public string Signature { get; set; }

        public TILARequest(int powerId, int loanId, Contract cont, string tila, string sign)
        {
            PowerID = powerId;
            LoanID = loanId;
            Contract = cont;
            TILA = tila;
            Signature = sign;
        }
    }
}
