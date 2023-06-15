using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetPayAdvance.LoanManagement.Infrastructure.Helpers;

public static class ResponseHelper
{
    public static async Task ThrowIfInvalidResponse(HttpResponseMessage res)
    {
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException rhe)
        {
            if (rhe.StatusCode == HttpStatusCode.NotFound)
            {
                throw new HttpRequestException("Not Found", rhe, rhe.StatusCode);
            }
            // ParseMessage out
            throw new HttpRequestException(await res.Content.ReadAsStringAsync(), rhe, rhe.StatusCode);
        }
    }
}