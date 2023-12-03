namespace SupportLibrary.Models
{
    public interface IRoutingModel
    {
        int Id { get; set; }
        int MachineId { get; set; }
        int ProcessTimeInSeconds { get; set; }
        int ProductId { get; set; }
        int SetupTimeInSeconds { get; set; }
        int StepId { get; set; }
    }
}