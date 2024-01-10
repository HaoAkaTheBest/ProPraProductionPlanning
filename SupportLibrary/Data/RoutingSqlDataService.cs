using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.Data
{
    public class RoutingSqlDataService : IRoutingSqlDataService
    {
        private readonly ISqlDataAccess _dataAccess;

        public RoutingSqlDataService(ISqlDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task CreateRouting(IRoutingModel routing)
        {
            var p = new
            {
                routing.MachineId,
                routing.ProductId,
                routing.StepId,
                routing.SetupTimeInSeconds,
                routing.ProcessTimeInSeconds
            };

            await _dataAccess.SaveData("dbo.spRoutings_Create", p, "SQLDB");
            //try
            //{
            //    await _dataAccess.SaveData("dbo.spRoutings_Create", p, "SQLDB");
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
        }

        public async Task<List<IRoutingModel>> ReadRouting()
        {
            var routing = await _dataAccess.LoadData<RoutingModel, dynamic>("dbo.spRoutings_Read",
                                                                          new { },
                                                                          "SQLDB");

            return routing.ToList<IRoutingModel>();
        }

        public async Task<List<IRoutingModel>> ReadRoutingForOneProduct(int productId)
        {
            var routing = await _dataAccess.LoadData<RoutingModel, dynamic>("dbo.spRoutings_ReadForOneProduct",
                                                                          new { ProductId = productId },
                                                                          "SQLDB");

            return routing.ToList<IRoutingModel>();
        }

        public async Task UpdateRouting(IRoutingModel routing)
        {
            await _dataAccess.SaveData("dbo.spRoutings_Update",
                routing, "SQLDB");
        }
        public async Task DeleteRouting(int id)
        {
            await _dataAccess.SaveData("dbo.spRoutings_Delete",
                new { Id = id }, "SQLDB");
        }

        public async Task<List<IRoutingModel>> SearchRouting(string searchTerm)
        {
            var routing = await _dataAccess.LoadData<RoutingModel, dynamic>("dbo.spRoutings_Search",
                                                                          new { SearchTerm = searchTerm },
                                                                          "SQLDB");

            return routing.ToList<IRoutingModel>();
        }
    }
}
