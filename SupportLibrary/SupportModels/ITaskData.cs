namespace SupportLibrary.SupportModels
{
    public interface ITaskData
    {
        public string DurationUnit { get; set; }
        DateTime Deadline { get; set; }
        string Duration { get; set; }
        DateTime EarliestStartDate { get; set; }
        DateTime EndDate { get; set; }
        DateTime OrderDate { get; set; }
        string ProductId { get; set; }
        int Progress { get; set; }
        string Status { get; set; }
        int TaskId { get; set; }
        string Note { get; set; }
    }
}