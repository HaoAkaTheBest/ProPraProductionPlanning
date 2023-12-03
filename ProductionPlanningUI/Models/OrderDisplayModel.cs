using SupportLibrary.Models;

namespace ProductionPlanningUI.Models
{
    public class OrderDisplayModel
    {
        public DateTime Deadline { get; set; }
        public DateTime EarliestStartDate { get; set; }
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductId { get; set; }
        public int Progress { get; set; }
        public int? ParentId { get; set; }
        public string Status { get; set; }
    }
}
