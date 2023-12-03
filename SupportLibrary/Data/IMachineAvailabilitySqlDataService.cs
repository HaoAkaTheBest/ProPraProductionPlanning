
namespace SupportLibrary.Data
{
    public interface IMachineAvailabilitySqlDataService
    {
        Task CreateMachineAvailability(IMachineAvailabilityModel availability);
        Task DeleteMachineAvailability(int id);
        Task<List<IMachineAvailabilityModel>> ReadMachineAvailability();
        Task<IMachineAvailabilityModel> ReadMachineAvailability(int id);
        Task<List<IMachineAvailabilityModel>> SearchMachineAvailability(string searchTerm);
        Task UpdateMachineAvailability(IMachineAvailabilityModel availability);
    }
}