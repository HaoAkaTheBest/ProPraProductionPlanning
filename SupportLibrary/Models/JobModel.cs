using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.Models;

public class JobModel
{
    public int OrderId { get; set; }
    public List<AufgabeModel> Tasks { get; set; } = new();
}
