using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Models.BillingStatements;
using NetPayAdvance.LoanManagement.Application.Models.Contracts;
using System.Text.Json;
using NetPayAdvance.LoanManagement.Infrastructure.Helpers;

namespace NetPayAdvance.LoanManagement.Infrastructure.Contracts.Services
{
    public class BaseContractService : IBaseContractService
    {
        private readonly HttpClient client;
        private readonly JsonOptions options;

        public BaseContractService(HttpClient c, IOptions<JsonOptions> options)
        {
            this.options = options.Value;
            client = c ?? throw new ArgumentNullException(nameof(c));
        }

        public async Task<T> GetContract<T>(string contractType, CancellationToken t = default)
        {
            try
            {
                var response = await client.GetAsync("templates?id=" + contractType,t);
                await ResponseHelper.ThrowIfInvalidResponse(response);
                return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(t));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<T> BuildContract<T>(TILARequest c, CancellationToken t = default) 
        {
            try
            {
                var response = await client.PostAsync("build-contract", 
                    new StringContent(JsonSerializer.Serialize(c), Encoding.UTF8, "application/json"), t);
                await ResponseHelper.ThrowIfInvalidResponse(response);
                return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(t), new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public async Task<T> SignContract<T>(TILARequest c, CancellationToken t = default) 
        {
            try
            {
                var response = await client.PostAsync("sign-contract", 
                    new StringContent(JsonSerializer.Serialize(c), Encoding.UTF8, "application/json"), t);
                await ResponseHelper.ThrowIfInvalidResponse(response);
                return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(t), new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<T> PostBillingStatement<T>(BillingStatementRequest c, CancellationToken t = default)
        {
            try
            {
                var response = await client.PostAsync("create-billing-statement",
                    new StringContent(JsonSerializer.Serialize(c, options.JsonSerializerOptions), Encoding.UTF8, "application/json"), t);
                await ResponseHelper.ThrowIfInvalidResponse(response);
                return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(t));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

