using NetPayAdvance.LoanManagement.Application.Users;

namespace NetPayAdvance.LoanManagement.Application.Abstractions;

public interface IUserService
{
    User GetUser();
}