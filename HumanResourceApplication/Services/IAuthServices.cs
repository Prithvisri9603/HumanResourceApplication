namespace HumanResourceApplication.Services
{
    public interface IAuthServices
    {
        string Authenticate(string username, string password);
    }
}
