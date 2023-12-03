using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.Models
{
    public class MachineModel : IMachineModel
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public double Effectivity { get; set; }
        public int MachineAlternativityGroup { get; set; }

    }
}
