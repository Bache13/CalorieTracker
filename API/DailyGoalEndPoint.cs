using System.Security.Claims;
using CaloryTracker.Data;
using Microsoft.EntityFrameworkCore;

public static class DailyGoalEndPoint
{
    public static void MapDailyGoal(this WebApplication app)
    {
        app.MapGet("/dailygoals/today", async (CalDbContext Dbcontext, HttpContext context, DateTime? date) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Results.Unauthorized();
            }

            var targetDate = (date ?? DateTime.UtcNow).Date;

            var dailyGoal = await Dbcontext.DailyGoals
                .FirstOrDefaultAsync(g => g.Date.Date == targetDate && g.UserId == userId);

            if (dailyGoal == null)
            {
                return Results.NotFound();
            }

            var dto = new DailyGoalDto
            {
                UserId = dailyGoal.UserId,
                Date = dailyGoal.Date,
                KcalGoal = dailyGoal.KcalGoal,
                ProteinGoal = dailyGoal.ProteinGoal,
                FatGoal = dailyGoal.FatGoal,
                CarbsGoal = dailyGoal.CarbsGoal,
                FiberGoal = dailyGoal.FiberGoal
            };

            return Results.Ok(dto);
        })
        .RequireAuthorization();

        app.MapPost("/dailygoals", async (CalDbContext DbContext, DailyGoalCreateDto dto, HttpContext context) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Results.Unauthorized();
            }

            var today = DateTime.UtcNow.Date;

            var existingGoal = await DbContext.DailyGoals
                .FirstOrDefaultAsync(g => g.Date.Date == today && g.UserId == userId);

            if (existingGoal != null)
            {
                return Results.Conflict("A goal already exists for today.");
            }

            var goal = new DailyGoal
            {
                UserId = userId,
                Date = today,
                KcalGoal = dto.KcalGoal,
                ProteinGoal = dto.ProteinGoal,
                FatGoal = dto.FatGoal,
                CarbsGoal = dto.CarbsGoal,
                FiberGoal = dto.FiberGoal
            };

            await DbContext.DailyGoals.AddAsync(goal);
            await DbContext.SaveChangesAsync();

            return Results.Ok(goal);
        })
        .RequireAuthorization();

        app.MapPut("/dailygoals/{date}/", async (CalDbContext DbContext, DateTime date, DailyGoalUpdateDto dto, HttpContext context) =>
        {

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Results.Unauthorized();
            }

            var goal = await DbContext.DailyGoals
                .FirstOrDefaultAsync(d => d.Date.Date == date.Date && d.UserId == userId);

            if (goal == null)
            {
                return Results.NotFound();
            }

            if (dto.KcalGoal.HasValue) goal.KcalGoal = dto.KcalGoal.Value;
            if (dto.ProteinGoal.HasValue) goal.ProteinGoal = dto.ProteinGoal.Value;
            if (dto.FatGoal.HasValue) goal.FatGoal = dto.FatGoal.Value;
            if (dto.CarbsGoal.HasValue) goal.CarbsGoal = dto.CarbsGoal.Value;
            if (dto.FiberGoal.HasValue) goal.FiberGoal = dto.FiberGoal.Value;

            await DbContext.SaveChangesAsync();

            return Results.Ok();
        })
        .RequireAuthorization();

        app.MapDelete("/dailygoals/today/", async (CalDbContext DbContext, DateTime date, HttpContext context) =>
        {

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Results.Unauthorized();
            }

            var today = DateTime.UtcNow.Date;

            var dailyGoal = await DbContext.DailyGoals
                .FirstOrDefaultAsync(d => d.Date.Date == today && d.UserId == userId);

            if (dailyGoal == null)
            {
                return Results.NotFound();
            }

            DbContext.DailyGoals.Remove(dailyGoal);
            await DbContext.SaveChangesAsync();

            return Results.Ok("Today's daily goal has been deleted.");
        })
        .RequireAuthorization();
    }
}