using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Users;

namespace NetPayAdvance.LoanManagement.Infrastructure.Users;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _context;  
    public UserService(IHttpContextAccessor context)  
    {  
        _context = context;  
    }  
    
    public User GetUser()
    {
        var identity = _context.HttpContext.User?.Identity;
        var user = ((ClaimsIdentity) identity).Claims.FirstOrDefault(f => f.Type.ToLower() == "teller")?.Value ?? "ILM";
        return new User(user);
    }
}