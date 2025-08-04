using System.Security.Cryptography;

namespace CYR.Services;

public class PasswordHasherService : IPasswordHasherService
{
    public string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[16];
        RandomNumberGenerator.Fill(salt);

        // Hash the password and salt together
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(20); // 20 is the size of the hash output

        // Combine salt and hash
        byte[] hashBytes = new byte[36]; // 16 (salt) + 20 (hash)
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        // Convert to base64
        string hashedPassword = Convert.ToBase64String(hashBytes);

        return hashedPassword;
    }

    public bool VerifyPassword(string savedPasswordHash, string password)
    {
        // Convert saved password hash to bytes
        byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

        // Extract salt from saved hash
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        // Compute hash using the same salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(20);

        // Compare hashes
        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
            {
                return false;
            }
        }
        return true;
    }
}
