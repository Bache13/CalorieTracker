using CaloryTracker.Data;
using Microsoft.EntityFrameworkCore;

public static class DailyGoalEndPoint
{
    public static void MapDailyGoal(this WebApplication app)
    {
        app.MapGet("/dailygoals/{date}", async (CalDbContext Dbcontext, DateTime date, int userId) =>
        {
            if (date.Kind == DateTimeKind.Unspecified)
            {
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }

            var dailyGoal = await Dbcontext.DailyGoals
                .FirstOrDefaultAsync(g => g.Date.Date == date.ToUniversalTime().Date);

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

        });

        app.MapPost("/dailygoals", async (CalDbContext DbContext, DailyGoalCreateDto dto) =>
        {
            var today = DateTime.UtcNow.Date;

            var existingGoal = await DbContext.DailyGoals
                .FirstOrDefaultAsync(g => g.Date.Date == today);

            if (existingGoal != null)
            {
                return Results.Conflict("A goal already exists for today.");
            }

            var goal = new DailyGoal
            {
                UserId = dto.UserId,
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
        });

        app.MapPut("/dailygoals/{date}/", async (CalDbContext DbContext, DateTime date, DailyGoalUpdateDto dto) =>
        {
            var goal = await DbContext.DailyGoals
                .FirstOrDefaultAsync(d => d.Date.Date == date.Date);

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
        });

        app.MapDelete("/dailygoals/{date}/", async (CalDbContext DbContext, DateTime date) =>
        {
            var dailyGoal = await DbContext.DailyGoals
                .FirstOrDefaultAsync(d => d.Date.Date == date.Date);

            if (dailyGoal == null)
            {
                return Results.NotFound();
            }

            DbContext.Remove(dailyGoal);

            await DbContext.SaveChangesAsync();

            return Results.Ok();
        });
    }
}