namespace SupportLibrary.Data
{
    public class OrderSqlDataService : IOrderSqlDataService
    {
        private readonly ISqlDataAccess _dataAccess;

        public OrderSqlDataService(ISqlDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task CreateOrder(IOrderModel order)
        {
            var p = new
            {
                order.Id,
                order.ProductId,
                order.Deadline,
                order.EarliestStartDate,
                order.OrderDate
            };

            await _dataAccess.SaveData("dbo.spOrders_Create", p, "SQLDB");
            //try
            //{
            //    await _dataAccess.SaveData("dbo.spOrders_Create", p, "SQLDB");
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
        }

        public async Task<List<IOrderModel>> ReadOrder()
        {
            var order = await _dataAccess.LoadData<OrderModel, dynamic>("dbo.spOrders_Read",
                                                                          new { },
                                                                          "SQLDB");

            return order.ToList<IOrderModel>();
        }


        public async Task<IOrderModel> ReadOrder(int id)
        {
            var order = await _dataAccess.LoadData<OrderModel, dynamic>("dbo.spOrders_ReadOne",
                                                                          new { Id = id },
                                                                          "SQLDB");

            return order.FirstOrDefault();
        }

        public async Task UpdateOrder(IOrderModel order)
        {
            await _dataAccess.SaveData("dbo.spOrders_Update",
                order, "SQLDB");
        }
        public async Task DeleteOrder(int id)
        {
            await _dataAccess.SaveData("dbo.spOrders_Delete",
                new { Id = id }, "SQLDB");
        }

        public async Task UpdateOrderDeadline(int orderId, DateTime newDeadline)
        {
            await _dataAccess.SaveData("dbo.spOrders_UpdateDeadline", new { Id = orderId, Deadline = newDeadline }, "SQLDB");
        }

        public async Task UpdateOrderEarliestStartDate(int orderId, DateTime earliestStartDate)
        {
            await _dataAccess.SaveData("dbo.spOrders_UpdateEarliestStartDate", new { Id = orderId, EarliestStartDate = earliestStartDate }, "SQLDB");
        }

        public async Task<List<IOrderModel>> SearchOrder(string searchTerm)
        {
            var order = await _dataAccess.LoadData<OrderModel, dynamic>("dbo.spOrders_Search",
                                                                          new { SearchTerm = searchTerm },
                                                                          "SQLDB");

            return order.ToList<IOrderModel>();
        }
    }
}
