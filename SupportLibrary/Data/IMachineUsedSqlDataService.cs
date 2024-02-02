
namespace SupportLibrary.Data
{
    public interface IMachineUsedSqlDataService
    {
        Task CreateMachineUsed(int orderId ,int machineId, DateTime startTime, DateTime endTime);
        Task CreateMachineUsedOptimized(int orderId, int machineId, int start, int duration, int taskId);
        Task DeleteAllData();
        Task DeleteAllMachineUsed();
        Task<IMachineUsedModel> ReadMachineUsedInThisTime(int machineId, DateTime startTime, DateTime endTime);
        Task<List<IMachineUsedOptimizedModel>> ReadMachineUsedOptimized();
    }
}