using Api.Workbench;
using Microsoft.PowerShell.Commands;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Workbench.PowerSharp
{
    public static class ExtensionMethods
    {
        public static string ConvertToJson(this Collection<PSObject> psobjects, int maxDepth = 10, bool enumAsStrings = true, bool compressOutput = true)
        {
            JsonObject.ConvertToJsonContext _json_context = new JsonObject.ConvertToJsonContext(maxDepth, enumAsStrings, compressOutput);
            return JsonObject.ConvertToJson(psobjects, _json_context);
        }
        public static JsonPSExecutionResult ConvertToJsonPSExecutionResult(this PSExecutionResult psExecutionResult)
        {
            var resultsInJson = psExecutionResult.Results.ConvertToJson();
            return new JsonPSExecutionResult() { ComputerName = psExecutionResult.ComputerName, Results = resultsInJson, HadErrors = psExecutionResult.HadErrors, Errors = psExecutionResult.Errors };
        }
    }
}