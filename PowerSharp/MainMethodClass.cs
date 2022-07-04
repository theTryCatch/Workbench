using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerShell.Commands;
using Newtonsoft.Json;

namespace Workbench.PowerSharp
{
    public class MainMethodClass
    {
        public static void Main()
        {
            var a = new PSCode("dc1", "get-service dns* | select Name, displayname", CodeType.Script);
            a.Invoke();
        }
    }
}
