using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.Models
{
    public class MachineAvailabilityModel : IMachineAvailabilityModel
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public DateTime PauseStartDate { get; set; }
        public DateTime PauseEndDate { get; set; }
        public string Description { get; set; }

    }
}
