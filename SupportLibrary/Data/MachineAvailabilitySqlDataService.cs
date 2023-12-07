namespace SupportLibrary.Data
{
    public class MachineAvailabilitySqlDataService : IMachineAvailabilitySqlDataService
    {
        private readonly ISqlDataAccess _dataAccess;

        public MachineAvailabilitySqlDataService(ISqlDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task CreateMachineAvailability(IMachineAvailabilityModel availability)
        {
            var p = new
            {
                availability.MachineId,
                availability.PauseStartDate,
                availability.PauseEndDate,
                availability.Description
                
            };
            try
            {
                await _dataAccess.SaveData("dbo.spMachineAvailabilities_Create", p, "SQLDB");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<IMachineAvailabilityModel>> ReadMachineAvailability()
        {
            var availability = await _dataAccess.LoadData<MachineAvailabilityModel, dynamic>("dbo.spMachineAvailabilities_Read",
                                                                          new { },
                                                                          "SQLDB");

            return availability.ToList<IMachineAvailabilityModel>();
        }

        public async Task<IMachineAvailabilityModel> ReadMachineAvailabilityForProduction(int machineId, DateTime startDate)
        {
            var availability = await _dataAccess.LoadData<MachineAvailabilityModel, dynamic>("dbo.spMachineAvailabilities_ReadAvailabilityForProduction",
                                                                          new { MachineId = machineId, StartDate = startDate },
                                                                          "SQLDB");

            return availability.FirstOrDefault();
        }

        public async Task UpdateMachineAvailability(IMachineAvailabilityModel availability)
        {
            await _dataAccess.SaveData("dbo.spMachineAvailabilities_Update",
                availability, "SQLDB");
        }
        public async Task DeleteMachineAvailability(int id)
        {
            await _dataAccess.SaveData("dbo.spMachineAvailabilities_Delete",
                new { Id = id }, "SQLDB");
        }

        public async Task<List<IMachineAvailabilityModel>> SearchMachineAvailability(string searchTerm)
        {
            var availability = await _dataAccess.LoadData<MachineAvailabilityModel, dynamic>("dbo.spMachineAvailabilities_Search",
                                                                          new { SearchTerm = searchTerm },
                                                                          "SQLDB");

            return availability.ToList<IMachineAvailabilityModel>();
        }
    }
}
