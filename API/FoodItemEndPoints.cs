using CaloryTracker.Data;
using Microsoft.EntityFrameworkCore;

public static class FoodItemEndpoints
{
    public static void MapFoodItemApi(this WebApplication app)
    {

        app.MapGet("/fooditems", async (CalDbContext DbContext) =>
        {
            try
            {
                var food = await DbContext.FoodItems.ToListAsync();
                return Results.Ok(food);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not retrieve data: {ex.Message}");
                return Results.NotFound();
            }
        });

        app.MapPost("/fooditems", async (CalDbContext DbContext, FoodItemDto dto) =>
        {
            var foodItem = new FoodItem
            {
                Name = dto.Name,
                Kcal = dto.Kcal,
                Fat = dto.Fat,
                Protein = dto.Protein,
                Carbs = dto.Carbs,
                Fiber = dto.Fiber

            };

            await DbContext.FoodItems.AddAsync(foodItem);
            await DbContext.SaveChangesAsync();

            return Results.Created($"/fooditems/{foodItem.Id}", foodItem);
        });

        app.MapPut("/fooditems{id}/", async (CalDbContext DbContext, int id, FoodItemUpdateDto dto) =>
        {
            var foodItem = await DbContext.FoodItems.FindAsync(id);

            if (foodItem == null)
            {
                return Results.NotFound();
            }

            foodItem.Name = dto.Name;
            foodItem.Kcal = dto.Kcal;
            foodItem.Fat = dto.Fat;
            foodItem.Protein = dto.Protein;
            foodItem.Carbs = dto.Carbs;
            foodItem.Fiber = dto.Fiber;

            await DbContext.SaveChangesAsync();

            return Results.Ok(foodItem);
        });

        app.MapDelete("/fooditems{id}/", async (CalDbContext DbContext, int id) =>
        {
            var foodItem = await DbContext.FoodItems.FindAsync(id);

            if (foodItem == null)
            {
                return Results.NotFound();
            }

            DbContext.Remove(foodItem);

            await DbContext.SaveChangesAsync();

            return Results.Ok();
        });
    }
}