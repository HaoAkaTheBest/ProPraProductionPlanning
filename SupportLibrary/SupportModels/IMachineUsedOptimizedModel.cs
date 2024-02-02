namespace SupportLibrary.SupportModels
{
    public interface IMachineUsedOptimizedModel
    {
        int Duration { get; set; }
        int MachineId { get; set; }
        int OrderId { get; set; }
        int Start { get; set; }
        int TaskId { get; set; }
    }
}