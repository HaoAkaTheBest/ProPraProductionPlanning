using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.Data
{
    public class MachineUsedSqlDataService : IMachineUsedSqlDataService
    {
        private readonly ISqlDataAccess _dataAccess;

        public MachineUsedSqlDataService(ISqlDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task CreateMachineUsed(int machineId, DateTime startTime, DateTime endTime)
        {
            var p = new
            {
                MachineId = machineId,
                StartTime = startTime,
                EndTime = endTime

            };
            try
            {
                await _dataAccess.SaveData("dbo.spMachineUsed_Create", p, "SQLDB");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IMachineUsedModel> ReadMachineUsedInThisTime(int machineId, DateTime startTime)
        {
            var machineUsed = await _dataAccess.LoadData<MachineUsedModel, dynamic>("dbo.spMachineUsed_CheckIfBeingUsed",
                                                                                     new { MachineId = machineId, StartTime = startTime },
                                                                                     "SQLDB");

            return machineUsed.FirstOrDefault();
        }
    }
}
