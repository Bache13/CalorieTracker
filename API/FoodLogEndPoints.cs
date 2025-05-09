using CaloryTracker.Data;
using Microsoft.EntityFrameworkCore;


public static class FoodLogEndpoints
{
    public static void MapFoodLogApi(this WebApplication app)
    {
        app.MapGet("/foodlogs", async (CalDbContext DbContext, int userId) =>
        {

            try
            {
                var foodLogs = await DbContext.FoodLogs
                    .Where(fl => fl.UserId == userId)
                    .Include(fl => fl.FoodItem)
                    .Select(fl => new FoodLogResposnseDto
                    {
                        Id = fl.Id,
                        FoodName = fl.FoodItem.Name,
                        PortionSize = fl.PortionSize,
                        LogDate = fl.LogDate.ToString("yyyy-mm-dd HH:mm")
                    })
                    .ToListAsync();

                return Results.Ok(foodLogs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can not retrieve data {ex.Message}");
                return Results.NotFound();
            }
        })
        .RequireAuthorization();

        app.MapPost("/foodlogs", async (CalDbContext DbContext, FoodLogCreateDto dto) =>
        {
            var foodLogs = new FoodLog
            {
                UserId = dto.UserId, // <---- ADDED THIS 
                FoodItemId = dto.FoodItemId,
                PortionSize = dto.PortionSize,
                LogDate = dto.LogDate
            };

            await DbContext.FoodLogs.AddAsync(foodLogs);
            await DbContext.SaveChangesAsync();

            return Results.Created($"/foodlogs/{foodLogs.Id}", foodLogs);
        });

        app.MapPut("/foodlogs/{id}/", async (CalDbContext DbContext, int id, FoodLogUpdateDto dto) =>
        {
            var foodLogs = await DbContext.FoodLogs.FindAsync(id);

            if (foodLogs == null)
            {
                return Results.NotFound();
            }

            foodLogs.FoodItemId = dto.FoodItemId;
            foodLogs.PortionSize = dto.PortionSize;
            foodLogs.LogDate = dto.LogDate;

            await DbContext.SaveChangesAsync();

            return Results.Ok(foodLogs);
        });

        app.MapDelete("/foodlogs/{id}/", async (CalDbContext Dbcontext, int id) =>
        {
            var foodLog = await Dbcontext.FoodLogs.FindAsync(id);

            if (foodLog == null)
            {
                return Results.NotFound();
            }

            Dbcontext.Remove(foodLog);

            await Dbcontext.SaveChangesAsync();

            return Results.Ok();
        });

        // Add Id
        app.MapGet("/foodlogs/summary", async (CalDbContext DbContext) =>
        {
            var today = DateTime.UtcNow.Date;

            var goal = await DbContext.DailyGoals
                .FirstOrDefaultAsync(g => g.Date.Date == today);

            if (goal == null)
            {
                return Results.NotFound();
            }

            var foodLogs = await DbContext.FoodLogs
                .Include(fl => fl.FoodItem)
                .Where(fl => fl.LogDate.Date == today)
                .ToListAsync();

            var totalKcal = foodLogs.Sum(fl => fl.PortionSize / 100 * fl.FoodItem.Kcal);
            var totalProtein = foodLogs.Sum(fl => fl.PortionSize / 100 * fl.FoodItem.Protein);
            var totalFat = foodLogs.Sum(fl => fl.PortionSize / 100 * fl.FoodItem.Fat);
            var totalCarbs = foodLogs.Sum(fl => fl.PortionSize / 100 * fl.FoodItem.Carbs);
            var totalFiber = foodLogs.Sum(fl => fl.PortionSize / 100 * fl.FoodItem.Fiber);

            var summary = new FoodLogDailySummaryDto
            {
                TotalKcal = totalKcal,
                KcalGoal = goal.KcalGoal,

                TotalProtein = totalProtein,
                ProteinGoal = goal.ProteinGoal,

                TotalFat = totalFat,
                FatGoal = goal.FatGoal,

                TotalCarbs = totalCarbs,
                CarbsGoal = goal.CarbsGoal,

                TotalFiber = totalFiber,
                FiberGoal = goal.FiberGoal
            };

            return Results.Ok(summary);
        });
    }
}

