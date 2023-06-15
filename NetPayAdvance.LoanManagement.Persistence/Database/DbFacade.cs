using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Persistence.Database
{
    #nullable enable
    public class DbFacade : IDbFacade
    {
        private readonly IDbConnection connection; 
       
        public DbFacade(IConfiguration configuration) => connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? tr = null)
        {
            return (await connection.QueryAsync<T>(sql, param, tr)).AsList();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? tr = null)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, tr);
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? tr = null)
        {
            return await connection.QuerySingleOrDefaultAsync<T>(sql, param, tr);
        }

        public async Task<IReadOnlyList<T>> QueryMultipleAsync<T>(string sql, object? param = null, IDbTransaction? tr = null)
        {
            var query = await connection.QueryMultipleAsync(sql, param, tr, commandType: CommandType.StoredProcedure);
            await query.ReadAsync();
            var result = await query.ReadAsync<T>();
            return result.AsList();
        }

        public async void ExecuteAsync(string sql, object? param = null, IDbTransaction? tr = null, CommandType ct = CommandType.Text)
        {
            await connection.ExecuteAsync(sql, param, tr, commandType: ct).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<T>> QueryProcAsync<T>(string proc, object? param = null, IDbTransaction? tr = null)
        {
            return (await connection.QueryAsync<T>(proc, param, tr, commandType: CommandType.StoredProcedure)).AsList();
        }
    }
}
