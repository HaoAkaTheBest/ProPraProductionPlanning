
namespace SupportLibrary.Models
{
    public interface IMachineAvailabilityModel
    {
        string Description { get; set; }
        int Id { get; set; }
        int MachineId { get; set; }
        DateTime PauseEndDate { get; set; }
        DateTime PauseStartDate { get; set; }
    }
}