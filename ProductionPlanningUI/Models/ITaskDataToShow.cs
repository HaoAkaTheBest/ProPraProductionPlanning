
namespace ProductionPlanningUI.Models
{
    public interface ITaskDataToShow
    {
        DateTime Deadline { get; set; }
        string Duration { get; set; }
        string DurationUnit { get; set; }
        DateTime EndDate { get; set; }
        string Note { get; set; }
        DateTime OrderDate { get; set; }
        int? ParentId { get; set; }
        int Progress { get; set; }
        DateTime StartDate { get; set; }
        string Status { get; set; }
        int TaskId { get; set; }
        string TaskName { get; set; }
    }
}