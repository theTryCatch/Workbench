using Microsoft.AspNetCore.Mvc;
using Workbench.PowerSharp;
using Newtonsoft.Json;
using Microsoft.PowerShell.Commands;
using System.Resources;

namespace Api.Workbench
{
    [Route("WinServices")]
    [ApiController]
    public class WinServicesController : Controller
    {
        [HttpGet("{Computer}")]
        public ActionResult<JsonPSExecutionResult> Get(string Computer)
        {
            return Ok(new PSCode(Computer, $"C:\\Users\\admin\\Desktop\\PowerShellScripts\\Get-WinServices.ps1", CodeType.ScriptFile).Invoke().ConvertToJsonPSExecutionResult());
        }
        [HttpGet("{Computer}/{ServiceName}")]
        public ActionResult<JsonPSExecutionResult> Get(string Computer, string ServiceName)
        {
            var paramsDict = new Dictionary<string, object>();
            paramsDict.Add("ServiceName", ServiceName);
            return Ok(new PSCode(Computer, $"C:\\Users\\admin\\Desktop\\PowerShellScripts\\Get-WinServices.ps1", CodeType.ScriptFile,paramsDict).Invoke().ConvertToJsonPSExecutionResult());
        }
        [HttpGet("{Computer}/{ServiceName}/{ServiceStatus}")]
        public ActionResult<JsonPSExecutionResult> Get(string Computer, string ServiceName, string ServiceStatus)
        {
            var paramsDict = new Dictionary<string, object>();
            paramsDict.Add("ServiceName", ServiceName);
            paramsDict.Add("ServiceStatus", ServiceStatus);
            return Ok(new PSCode(Computer, $"C:\\Users\\admin\\Desktop\\PowerShellScripts\\Get-WinServices.ps1", CodeType.ScriptFile, paramsDict).Invoke().ConvertToJsonPSExecutionResult());
        }
    }
}
