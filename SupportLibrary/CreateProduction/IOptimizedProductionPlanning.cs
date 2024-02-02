
namespace SupportLibrary.CreateProduction
{
    public interface IOptimizedProductionPlanning
    {
        Task<List<ITaskData>> CreateOptimizedProductionPlan();
        Task<string> CreateProductionPlan();
    }
}