
namespace SupportLibrary.Models
{
    public interface IOrderModel
    {
        DateTime Deadline { get; set; }
        DateTime EarliestStartDate { get; set; }
        int Id { get; set; }
        DateTime OrderDate { get; set; }
        int ProductId { get; set; }
    }
}