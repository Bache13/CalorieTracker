using CaloryTracker.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

public static class UserEndpoints
{
        app.MapGet("/users", async (CalDbContext DbContext, HttpContext context) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim?.Value, out int userId))
            {
                return Results.Unauthorized();
            }

            var user = await DbContext.User.FindAsync(userId);

            if (user == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(user);
        })
        .RequireAuthorization();

        app.MapPost("/users/register/", async (CalDbContext DbContext, UserRegisterDto dto) =>
        {
            if (DbContext.User.Any(u => u.Email == dto.Email))
                return Results.BadRequest("Email already in use.");

            PasswordHasher.CreatePasswordHash(dto.Password, out string passwordHash, out string passwrodSalt);

            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwrodSalt,
                CreatedAt = DateTime.UtcNow,
                IsAdmin = false
            };

            await DbContext.User.AddAsync(user);
            await DbContext.SaveChangesAsync();

            return Results.Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.CreatedAt
            });

        });

        app.MapPost("/users/login", async (CalDbContext DbContext, IConfiguration config, UserLoginDto dto) =>
        {
            var user = await DbContext.User.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return Results.Unauthorized();
            }

            bool isPasswordValid = PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt);

            if (!isPasswordValid)
            {
                return Results.Unauthorized();
            }


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Results.Ok(new
            {
                token = jwt,
                user = new UserResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt
                }
            });
        });



        app.MapDelete("/users/{id}", async (HttpContext http, CalDbContext DbContext, int id) =>
        {
            var userToDelete = await DbContext.User.FindAsync(id);
            if (userToDelete == null)
            {
                return Results.NotFound("User does not exist.");
            }

            var requesterId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (requesterId == id.ToString())
            {
                return Results.BadRequest("You cannot delete yourself.");
            }

            DbContext.Remove(userToDelete);
            await DbContext.SaveChangesAsync();
            return Results.Ok();
        })
        .RequireAuthorization("AdminOnly");
    }
}