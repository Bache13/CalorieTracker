public class DailyGoal
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public float KcalGoal { get; set; }
    public float ProteinGoal { get; set; }
    public float FatGoal { get; set; }
    public float CarbsGoal { get; set; }
    public float FiberGoal { get; set; }
}