using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Models.BillingStatements;
using NetPayAdvance.LoanManagement.Application.Models.Contracts;

namespace NetPayAdvance.LoanManagement.Application.Abstractions
{
    public interface IBaseContractService
    {
        Task<T> BuildContract<T>(TILARequest c, CancellationToken t = default);
        
        Task<T> SignContract<T>(TILARequest c, CancellationToken t = default);

        Task<T> GetContract<T>(string contractType, CancellationToken t = default);

        Task<T> PostBillingStatement<T>(BillingStatementRequest c, CancellationToken t = default);
    }
}
