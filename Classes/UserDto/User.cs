using Microsoft.AspNetCore.Identity;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsAdmin { get; set; }
}