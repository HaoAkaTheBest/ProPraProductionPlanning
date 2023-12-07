using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.SupportModels
{
    public class OrderProcessModel : IOrderProcessModel
    {
        public DateTime EndDate { get; set; }
        public string Duration { get; set; }
        public int Progress { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }


    }
}
