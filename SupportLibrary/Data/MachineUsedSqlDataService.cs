using Dapper;
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

        public async Task CreateMachineUsed(int orderId,int machineId, DateTime startTime, DateTime endTime)
        {
            var p = new
            {
                OrderId = orderId,
                MachineId = machineId,
                StartTime = startTime,
                EndTime = endTime

            };
            await _dataAccess.SaveData("dbo.spMachineUsed_Create", p, "SQLDB");
        }

        public async Task CreateMachineUsedOptimized(int orderId, int machineId, int start, int duration, int taskId)
        {
            var p = new
            {
               MachineId = machineId,
               OrderId = orderId,
               Start = start,
               Duration = duration,
               TaskId = taskId
            };
            await _dataAccess.SaveData("dbo.spMachineUsed_CreateOptimized", p, "SQLDB");
        }

        public async Task<List<IMachineUsedOptimizedModel>> ReadMachineUsedOptimized()
        {
            var machineUsedOptimized = await _dataAccess.LoadData<MachineUsedOptimizedModel, dynamic>("dbo.spMachineUsed_ReadOptimized",
                                                                          new {},
            "SQLDB");

            return machineUsedOptimized.ToList<IMachineUsedOptimizedModel>();
        }

        public async Task<IMachineUsedModel> ReadMachineUsedInThisTime(int machineId, DateTime startTime, DateTime endTime)
        {
            var machineUsed = await _dataAccess.LoadData<MachineUsedModel, dynamic>("dbo.spMachineUsed_CheckIfBeingUsed",
                                                                                     new { MachineId = machineId, StartTime = startTime, EndTime = endTime },
                                                                                     "SQLDB");

            return machineUsed.FirstOrDefault();
        }

        public async Task DeleteAllData()
        {
            await _dataAccess.SaveData("dbo.spMachineUsed_DeleteAllData", new { }, "SQLDB");
        }

        public async Task DeleteAllMachineUsed()
        {
            await _dataAccess.SaveData("dbo.spMachineUsed_Delete", new { }, "SQLDB");
        }
    }
}
