

namespace SupportLibrary.Data
{
    public interface IMachineAvailabilitySqlDataService
    {
        Task CreateMachineAvailability(IMachineAvailabilityModel availability);
        Task DeleteMachineAvailability(int id);
        Task<List<IMachineAvailabilityModel>> ReadMachineAvailability();
        Task<IMachineAvailabilityModel> ReadMachineAvailabilityForProduction(int machineId, DateTime startDate);
        Task<List<IMachineAvailabilityModel>> SearchMachineAvailability(string searchTerm);
        Task UpdateMachineAvailability(IMachineAvailabilityModel availability);
    }
}