namespace CYR.Services
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string savedPasswordHash, string password);
    }
}