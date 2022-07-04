using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Workbench
{
    public class JsonPSExecutionResult
    {
        public string ComputerName { get; set; }
        public string? Results { get; set; }
        public bool HadErrors { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
