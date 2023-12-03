namespace SupportLibrary.Models
{
    public interface IMachineModel
    {
        string Description { get; set; }
        double Effectivity { get; set; }
        int Id { get; set; }
        int MachineAlternativityGroup { get; set; }
        string ShortName { get; set; }
    }
}