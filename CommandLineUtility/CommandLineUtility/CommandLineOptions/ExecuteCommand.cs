using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLineUtility.Services;

namespace CommandLineUtility.CommandLineOptions
{
    public class ExecuteCommand
    {
        private Dictionary<string, string> _requiredParameters;
        private Dictionary<string, string> _parameters;
        private string[] _args;

        public string ErrorMessage { get; set; }

        public ExecuteCommand(string[] args)
        {
            _args = args;

            _requiredParameters = new Dictionary<string, string>();
            _requiredParameters.Add("command", "");
        }

        public bool Execute()
        {
            try
            {
                if (_args.HasArgsWithValues(_requiredParameters.Keys.ToArray()))
                {
                    _parameters = _args.GetArguments();

                    Process process = new Process();
                    if (_args.HasArg("dir"))
                    {
                        process.StartInfo.WorkingDirectory = _parameters["dir"];
                    }
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = "/C " + _parameters["command"];
                    process.StartInfo.CreateNoWindow = false;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    string outPut = process.StandardOutput.ReadToEnd();

                    process.WaitForExit();
                    var exitCode = process.ExitCode;
                    process.Close();
                }
                else
                {
                    ErrorMessage = string.Format("Missing Required Parameter(s)");
                    return false;

                }
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format("Failure: {0}", ex.Message);
                return false;
            }
            return true;
        }

    }
}
