using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NetPayAdvance.LoanManagement.Persistence.Abstractions
{
    public interface IDbFacade
    {
        Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? tr = null);

        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? tr = null);

        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? tr = null);

        Task<IReadOnlyList<T>> QueryMultipleAsync<T>(string sql, object? param = null, IDbTransaction? tr = null);

        void ExecuteAsync(string sql, object? param = null, IDbTransaction? tr = null, CommandType ct = CommandType.Text);

        Task<IReadOnlyList<T>> QueryProcAsync<T>(string proc, object? param = null, IDbTransaction? tr = null);
    }
}
