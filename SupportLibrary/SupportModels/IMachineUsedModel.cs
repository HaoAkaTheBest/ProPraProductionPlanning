namespace SupportLibrary.SupportModels
{
    public interface IMachineUsedModel
    {
        DateTime EndTime { get; set; }
        int MachineId { get; set; }
        DateTime StartTime { get; set; }
    }
}