namespace SupportLibrary.SupportModels
{
    public interface IMachineUsedModel
    {
        int OrderId { get; set; }
        DateTime EndTime { get; set; }
        int MachineId { get; set; }
        DateTime StartTime { get; set; }
    }
}