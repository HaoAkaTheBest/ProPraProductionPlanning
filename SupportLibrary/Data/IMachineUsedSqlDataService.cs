namespace SupportLibrary.Data
{
    public interface IMachineUsedSqlDataService
    {
        Task CreateMachineUsed(int orderId ,int machineId, DateTime startTime, DateTime endTime);
        Task<IMachineUsedModel> ReadMachineUsedInThisTime(int machineId, DateTime startTime, DateTime endTime);
    }
}