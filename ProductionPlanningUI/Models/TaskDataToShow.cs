namespace ProductionPlanningUI.Models
{
    public class TaskDataToShow : ITaskDataToShow
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Duration { get; set; }
        public int Progress { get; set; }
        public string DurationUnit { get; set; }
        public int? ParentId { get; set; }

        //Extra informations
        public DateTime OrderDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}
