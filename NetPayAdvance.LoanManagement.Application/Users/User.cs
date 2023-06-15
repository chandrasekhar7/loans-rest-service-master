namespace NetPayAdvance.LoanManagement.Application.Users;

public class User
{
    public string Teller { get; }

    public User(string teller)
    {
        Teller = teller;
    }
}