using CaloryTracker.Data;
using Microsoft.EntityFrameworkCore;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/users/{id}", async (CalDbContext DbContext, int id) =>
        {
            var user = await DbContext.User.FindAsync(id);

            if (user == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(user);
        });

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

        app.MapPost("/users/login", async (CalDbContext DbContext, UserLoginDto dto) =>
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

            var response = new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return Results.Ok(response);
        });


        // Add IsAdmin flag when editing because it is not in when creating? 
        // Or should I keep it seperate and make another put/post to handle this?
        // app.MapPut("/users/{id}", async (CalDbContext DbContext) =>
        // {

        // });

        app.MapDelete("/users/{id}", async (CalDbContext DbContext, int id) =>
        {
            var user = await DbContext.User.FindAsync(id);

            if (user == null)
            {
                return Results.NotFound("User does not exist.");
            }

            if (!user.IsAdmin)
            {
                return Results.Unauthorized();
            }

            DbContext.Remove(user);
            await DbContext.SaveChangesAsync();
            return Results.Ok();
        });
    }
}