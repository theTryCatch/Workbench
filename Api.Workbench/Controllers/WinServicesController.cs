using Microsoft.AspNetCore.Mvc;
using Workbench.PowerSharp;
using Newtonsoft.Json;
using Microsoft.PowerShell.Commands;


namespace Api.Workbench
{
    [Route("WinServices")]
    [ApiController]
    public class WinServicesController : Controller
    {
        [HttpGet("{computer}")]
        public ActionResult<JsonPSExecutionResult> Get(string computer)
        {
            return Ok(new PSCode(computer, "C:\\Users\\public\\Get-WinServices.ps1", CodeType.ScriptFile).Invoke().ConvertToJsonPSExecutionResult());
        }

        [Route("WinServices/{computer}/{serviceDisplayName}")]
        [HttpGet]
        public ActionResult<JsonPSExecutionResult> Get(string computer, string serviceDisplayName)
        {
            var paramss = new Dictionary<string, object>();
            paramss.Add("DisplayName", serviceDisplayName);
            return Ok(new PSCode(computer, "C:\\Users\\public\\Get-WinServices.ps1", CodeType.ScriptFile, paramss).Invoke().ConvertToJsonPSExecutionResult());
        }
    }
}
