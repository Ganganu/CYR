using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CYR.Services;
public class LoginTokenService : ILoginTokenService
{
    private readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "CYR", "login.token");

    public void SaveToken(string username, string token)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        string data = $"{username}:{token}";
        byte[] encrypted = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(data), null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(_filePath, encrypted);
    }

    public (string Username, string Token)? LoadToken()
    {
        if (!File.Exists(_filePath))
            return null;

        byte[] encrypted = File.ReadAllBytes(_filePath);
        byte[] decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
        string[] parts = Encoding.UTF8.GetString(decrypted).Split(':');
        if (parts.Length != 2) return null;
        return (parts[0], parts[1]);
    }

    public void DeleteToken()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }
}
