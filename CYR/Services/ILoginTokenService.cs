// Services/LoginTokenService.cs
public interface ILoginTokenService
{
    void DeleteToken();
    (string Username, string Token)? LoadToken();
    void SaveToken(string username, string token);
}