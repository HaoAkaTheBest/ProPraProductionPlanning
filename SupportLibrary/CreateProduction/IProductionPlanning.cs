
namespace SupportLibrary.CreateProduction
{
    public interface IProductionPlanning
    {
        Task<List<ITaskData>> CreateProductionPlan();
    }
}