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

        public override bool Equals(object? obj)
        {
            if (obj == null||GetType() != obj.GetType())
            {
                return false;
            }

            RoutingModel otherRouting = (RoutingModel)obj;

            return ProductId == otherRouting.ProductId && StepId == otherRouting.StepId && MachineId == otherRouting.MachineId && SetupTimeInSeconds == otherRouting.SetupTimeInSeconds && ProcessTimeInSeconds == otherRouting.ProcessTimeInSeconds;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ ProductId.GetHashCode() ^ StepId.GetHashCode() ^ MachineId.GetHashCode() ^ SetupTimeInSeconds.GetHashCode() ^ ProcessTimeInSeconds.GetHashCode();
        }

    }
}
