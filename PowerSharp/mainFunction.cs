using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workbench.PowerSharp;

namespace PowerSharp
{
    public class mainFunction
    {
        public static void Main()
        {
            var b = new PSCode(Environment.MachineName, "get-service", CodeType.Script).Invoke();
        }
    }
}
