using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.SupportModels
{
    public class TaskData : ITaskData
    {
        public int TaskId { get; set; }
        //TaskName
        public string ProductId { get; set; }
        //StartDate
        public DateTime EarliestStartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Duration { get; set; }
        public int Progress { get; set; }
        public string DurationUnit { get; set; }

        //Extra informations
        public DateTime OrderDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}
