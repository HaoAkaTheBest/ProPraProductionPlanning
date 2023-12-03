using SupportLibrary.Models;

namespace SupportLibrary.Data
{
    public interface IOrderSqlDataService
    {
        Task CreateOrder(IOrderModel order);
        Task DeleteOrder(int id);
        Task<List<IOrderModel>> ReadOrder();
        Task<IOrderModel> ReadOrder(int id);
        Task<List<IOrderModel>> SearchOrder(string searchTerm);
        Task UpdateOrder(IOrderModel order);
    }
}