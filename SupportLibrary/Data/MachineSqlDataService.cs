using System.Net.Http.Headers;

namespace SupportLibrary.Data
{
    public class MachineSqlDataService : IMachineSqlDataService
    {
        private readonly ISqlDataAccess _dataAccess;

        public MachineSqlDataService(ISqlDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task CreateMachine(IMachineModel machine)
        {
            //string sql = @"insert into dbo.Machines(Id,ShortName,[Description],Effectivity,MachineAlternativityGroup) values(900,'BB','BBBBBBBB',0,0);";
            var p = new
            {
                machine.Id,
                machine.ShortName,
                machine.Description,
                machine.Effectivity,
                machine.MachineAlternativityGroup
            };

            try
            {
                await _dataAccess.SaveData("dbo.spMachines_Create", p, "SQLDB");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<IMachineModel>> ReadMachine()
        {
            var machine = await _dataAccess.LoadData<MachineModel, dynamic>("dbo.spMachines_Read", new { }, "SQLDB");

            return machine.ToList<IMachineModel>();
        }

        public async Task<IMachineModel> ReadMachine(int machineid)
        {
            var machine = await _dataAccess.LoadData<MachineModel, dynamic>("dbo.spMachines_ReadOneMachine", new { Id = machineid }, "SQLDB");

            return machine.FirstOrDefault();
        }

        public async Task UpdateMachine(IMachineModel machine)
        {
            await _dataAccess.SaveData("dbo.spMachines_Update",
                machine, "SQLDB");
        }
        public async Task DeleteMachine(int id)
        {
            await _dataAccess.SaveData("dbo.spMachines_Delete",
                new { Id = id }, "SQLDB");
        }

        public async Task<List<IMachineModel>> SearchMachine(string searchTerm)
        {
            var machine = await _dataAccess.LoadData<MachineModel, dynamic>("dbo.spMachines_Search",
                                                                          new { SearchTerm = searchTerm },
                                                                          "SQLDB");

            return machine.ToList<IMachineModel>();
        }
    }
}
