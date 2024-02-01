using SupportLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductionPlanningUI.Models;

public class DisplayOrderModel : IOrderModel
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public DateTime Deadline { get ; set ; }

    [Required]
    public DateTime EarliestStartDate { get ; set; }
    public DateTime OrderDate { get; set; }
}
