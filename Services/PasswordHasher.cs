using System.Security.Cryptography;
using System.Text;

public static class PasswordHasher
{
    public static void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = Convert.ToBase64String(hmac.Key);

        var combined = Encoding.UTF8.GetBytes(password + passwordSalt);
        var hash = hmac.ComputeHash(combined);
        passwordHash = Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));

        var combined = Encoding.UTF8.GetBytes(password + storedSalt);
        var hash = hmac.ComputeHash(combined);
        var hashToCheck = Convert.ToBase64String(hash);

        return hashToCheck == storedHash;
    }
}