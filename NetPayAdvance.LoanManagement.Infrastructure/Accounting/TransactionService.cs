using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Infrastructure.Helpers;
using Newtonsoft.Json;

namespace NetPayAdvance.LoanManagement.Infrastructure.Accounting
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient client;
        private readonly JsonOptions options;
    
        public TransactionService(HttpClient c,IOptions<JsonOptions> options)
        {
            this.options = options.Value;
            client = c ?? throw new ArgumentNullException(nameof(c));
        }
        
        public async Task<bool> PendingAch(int loanId, CancellationToken t = default)
        {
            var response = await client.GetAsync($"transactions/ach?loanId={loanId}&pending=true",t);
            await ResponseHelper.ThrowIfInvalidResponse(response);
            var d = await response.Content.ReadAsStringAsync(t);
            var transactions = JsonConvert.DeserializeObject<object[]>(d);
            return transactions?.Any() ?? false;
        }
    }
}

