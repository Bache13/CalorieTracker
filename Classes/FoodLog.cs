public class FoodLog
{
    public int Id { get; set; }
    public int FoodItemId { get; set; }
    public FoodItem FoodItem { get; set; }

    public float PortionSize { get; set; }
    public DateTime LogDate { get; set; }
}