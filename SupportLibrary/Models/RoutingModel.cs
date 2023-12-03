using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.Models
{
    public class RoutingModel : IRoutingModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int StepId { get; set; }
        public int MachineId { get; set; }
        public int SetupTimeInSeconds { get; set; }
        public int ProcessTimeInSeconds { get; set; }

    }
}
