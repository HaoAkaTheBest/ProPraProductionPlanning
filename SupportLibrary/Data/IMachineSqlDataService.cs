
namespace SupportLibrary.Data
{
    public interface IMachineSqlDataService
    {
        Task CreateMachine(IMachineModel machine);
        Task DeleteMachine(int id);
        Task<List<IMachineModel>> ReadMachine();
        Task<IMachineModel> ReadMachine(int id);
        Task<List<IMachineModel>> SearchMachine(string searchTerm);
        Task UpdateMachine(IMachineModel machine);
    }
}