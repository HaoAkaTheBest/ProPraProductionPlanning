using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.SupportModels;

public class MachineUsedOptimizedModel : IMachineUsedOptimizedModel
{
    public int OrderId { get; set; }
    public int MachineId { get; set; }
    public int Start { get; set; }
    public int Duration { get; set; }
    public int TaskId { get; set; }
}
