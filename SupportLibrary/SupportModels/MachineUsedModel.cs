using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.SupportModels
{
    public class MachineUsedModel : IMachineUsedModel
    {
        public int OrderId { get; set; }
        public int MachineId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
