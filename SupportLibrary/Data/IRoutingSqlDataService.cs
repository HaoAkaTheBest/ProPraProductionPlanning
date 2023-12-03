
namespace SupportLibrary.Data
{
    public interface IRoutingSqlDataService
    {
        Task CreateRouting(IRoutingModel routing);
        Task DeleteRouting(int id);
        Task<List<IRoutingModel>> ReadRouting();
        Task<IRoutingModel> ReadRouting(int id);
        Task<List<IRoutingModel>> SearchRouting(string searchTerm);
        Task UpdateRouting(IRoutingModel routing);
    }
}