using Microsoft.AspNetCore.Mvc;
using Workbench.PowerSharp;

namespace Api.Workbench
{
    [Route("WinServices")]
    [ApiController]
    public class WinServicesController : Controller
    {
        string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "PowerShell_Scripts", "Get-WinServices.ps1");
        [HttpGet("{Computer}")]
        public ActionResult<JsonPSExecutionResult> Get(string Computer)
        {
            return Ok(new PSCode(Computer, scriptPath, CodeType.Cmdlet).Invoke().ConvertToJsonPSExecutionResult());
        }
        [HttpGet("{Computer}/{ServiceName}")]
        public ActionResult<JsonPSExecutionResult> Get(string Computer, string ServiceName)
        {
            var paramsDict = new Dictionary<string, object>();
            paramsDict.Add("ServiceName", ServiceName);
            return Ok(new PSCode(Computer, scriptPath, CodeType.ScriptFile,paramsDict).Invoke().ConvertToJsonPSExecutionResult());
        }
        [HttpGet("{Computer}/{ServiceName}/{ServiceStatus}")]
        public ActionResult<JsonPSExecutionResult> Get(string Computer, string ServiceName, string ServiceStatus)
        {
            var paramsDict = new Dictionary<string, object>();
            paramsDict.Add("ServiceName", ServiceName);
            paramsDict.Add("ServiceStatus", ServiceStatus);
            return Ok(new PSCode(Computer, scriptPath, CodeType.ScriptFile, paramsDict).Invoke().ConvertToJsonPSExecutionResult());
        }
    }
}
