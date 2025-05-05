using Microsoft.EntityFrameworkCore;

namespace CaloryTracker.Data;

public class CalDbContext : DbContext
{
    public CalDbContext(DbContextOptions<CalDbContext> options) : base(options)
    {

    }

    public DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<FoodLog> FoodLogs { get; set; }
    public DbSet<DailyGoal> DailyGoals { get; set; }
}