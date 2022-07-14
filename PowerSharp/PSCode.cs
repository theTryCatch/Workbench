using System.Management.Automation.Runspaces;
using MSFT = System.Management.Automation;

namespace Workbench.PowerSharp
{
    public class PSCode
    {
        #region Public properties
        /// <summary>
        /// PSCode enables you to run any PowerShell command or a script on computer with the timeout feature.
        /// </summary>
        /// <remarks>
        /// This class implements PowerShell runspaces for execution and TPL (Task) for timeout feature.
        /// </remarks>
        public string ComputerName { get; }
        public string Code { get; }
        public CodeType CodeType { get; }
        public Dictionary<string, object>? Parameters { get; }
        public List<FileInfo>? ModulesTobeImported { get; }
        public uint TimeoutInSeconds { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Construct a new <c>PSCode</c>instance
        /// </summary>
        /// <param name="computername">Target computer name</param>
        /// <param name="code">PowerShell code (cmdlet, script, script file path)</param>
        /// <param name="codeType">What type of 'code' you provided?</param>
        /// <param name="parameters">specify list of parameters for the PowerShell 'code'</param>
        /// <param name="modulesTobeImported">Provide list of modules you wanted to import into the PowerShell environment.</param>
        /// <param name="timeoutInSeconds">Specify the timeout in seconds. Defaults to 30 seconds</param>
        public PSCode(
            string computername,
            string code, CodeType codeType,
            Dictionary<string, object>? parameters = null,
            List<FileInfo>? modulesTobeImported = null,
            uint timeoutInSeconds = 30
        )
        {
            #region Assigning the properties
            this.ComputerName = computername;
            this.Code = code;
            this.CodeType = codeType;
            this.Parameters = parameters;
            this.ModulesTobeImported = modulesTobeImported;
            this.TimeoutInSeconds = timeoutInSeconds;
            #endregion
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method invokes PowerShell synchronously and time out the execution based on the provided <see cref="TimeoutInSeconds"/> parameter.
        /// </summary>
        /// <remarks>This method is never expected to throw any errors</remarks>
        public PSExecutionResult Invoke()
        {
            Func<PSExecutionResult> func = new Func<PSExecutionResult>(IgnitePowerShellInvocation);

            try
            {
                PSExecutionResult? _executionResults;
                var _cts = new CancellationTokenSource();
                _cts.CancelAfter(TimeSpan.FromSeconds(this.TimeoutInSeconds));
                var _task = Task.Run(func, _cts.Token);
                using (_cts)
                {
                    _task.Wait(_cts.Token);
                    _executionResults = _task.Result;
                }

                //TODO: Find a way to handle this conditional disposement using 'using' blocks.
                if (_task.Status == TaskStatus.RanToCompletion || _task.Status == TaskStatus.Faulted || _task.Status == TaskStatus.Canceled)
                {
                    _task.Dispose();
                }

                return _executionResults;
            }
            catch (OperationCanceledException)
            {
                return new PSExecutionResult()
                {
                    ComputerName = this.ComputerName,
                    Errors = new List<string>() { ($"Execution timeout with in {this.TimeoutInSeconds} second(s)") },
                    HadErrors = true,
                    Results = null
                };
            }
            catch (Exception e)
            {
                return new PSExecutionResult()
                {
                    ComputerName = this.ComputerName,
                    Errors = new List<string>() {e.Message },
                    HadErrors = true,
                    Results = null
                };
            }
        }
        #endregion

        #region Private methods
        private PSExecutionResult IgnitePowerShellInvocation()
        {

            Runspace _runspace;
            MSFT.PowerShell _powershell;

            PSExecutionResult _executionResults;
            List<string> _errors = new List<string>();

            #region Runspace management
            var isLocalComputer = this.ComputerName.ToUpper() == Environment.MachineName.ToUpper() ? true : false;
            if (isLocalComputer)
                _runspace = RunspaceFactory.CreateRunspace();
            else
                _runspace = RunspaceFactory.CreateRunspace(new WSManConnectionInfo(new Uri($"http://{this.ComputerName}:{5985}/WSMAN")));
            #endregion

            #region PowerShell creation
            _powershell = MSFT.PowerShell.Create();
            _powershell.Runspace = _runspace;

            #region Adding the commands/ scripts to the PowerShell environment
            if (this.ModulesTobeImported != null)
            {
                foreach (var item in this.ModulesTobeImported)
                {
                    _powershell.AddCommand("Import-Module").AddArgument(item.ToString());
                }
            }
            if (this.CodeType == CodeType.Cmdlet)
                _powershell.AddCommand(this.Code);
            else if (this.CodeType == CodeType.Script)
                _powershell.AddScript(this.Code);
            else
                _powershell.AddScript(File.ReadAllText(this.Code));

            if (this.Parameters != null)
                _powershell.AddParameters(this.Parameters);
            #endregion

            #endregion
            try
            {
                using (_powershell)
                {
                    using (_runspace)
                    {
                        _runspace.Open();
                        var result = _powershell.Invoke();

                        //If the executed code is just a command then the PowerShell.Invoke() method throws an exception but
                        //if the code is a script and contains any errors then it will not indicate anyway about the exceptions.
                        //Hence we the logic is around HadErrors property data.
                        if (_powershell.HadErrors)
                        {
                            foreach (var item in (from error in _powershell.Streams.Error select error.Exception.Message))
                            {
                                _errors.Add(item);
                            }
                            _executionResults = new PSExecutionResult()
                            {
                                ComputerName = this.ComputerName,
                                HadErrors = true,
                                Results = null,
                                Errors = _errors
                            };
                        }
                        else
                        {
                            _executionResults = new PSExecutionResult()
                            {
                                ComputerName = this.ComputerName,
                                HadErrors = false,
                                Results = result,
                                Errors = null
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _errors.Add(e.ToString());
                _executionResults = new PSExecutionResult()
                {
                    ComputerName = this.ComputerName,
                    HadErrors = true,
                    Results = null,
                    Errors = _errors
                };
            }
            return _executionResults;
        }
        #endregion
    }
}