using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.Models
{
    public class OrderModel : IOrderModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime EarliestStartDate { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
