public class FoodLogDailySummaryDto
{
    public float TotalKcal { get; set; }
    public float KcalGoal { get; set; }
    public float RemainingKcal => KcalGoal - TotalKcal;

    public float TotalProtein { get; set; }
    public float ProteinGoal { get; set; }
    public float RemainingProtein => ProteinGoal - TotalProtein;

    public float TotalFat { get; set; }
    public float FatGoal { get; set; }
    public float RemainingFat => FatGoal - TotalFat;

    public float TotalCarbs { get; set; }
    public float CarbsGoal { get; set; }
    public float RemainingCarbs => CarbsGoal - TotalCarbs;

    public float TotalFiber { get; set; }
    public float FiberGoal { get; set; }
    public float RemainingFiber => FiberGoal - TotalFiber;
}