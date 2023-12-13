

namespace SupportLibrary.SupportModels
{
    public interface IOrderProcessModel
    {
        string Duration { get; set; }
        DateTime EndDate { get; set; }
        string Note { get; set; }
        int Progress { get; set; }
        string Status { get; set; }
        DateTime StartDate { get; set; }
    }
}