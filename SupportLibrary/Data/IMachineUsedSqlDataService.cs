namespace SupportLibrary.Data
{
    public interface IMachineUsedSqlDataService
    {
        Task CreateMachineUsed(int orderId ,int machineId, DateTime startTime, DateTime endTime);
        Task DeleteAllData();
        Task DeleteAllMachineUsed();
        Task<IMachineUsedModel> ReadMachineUsedInThisTime(int machineId, DateTime startTime, DateTime endTime);
    }
}