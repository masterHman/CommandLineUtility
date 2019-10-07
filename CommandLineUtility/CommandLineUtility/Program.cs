using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLineUtility.Services;

namespace CommandLineUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.HasArg("ReplaceInFiles"))
            {
                var rename = new CommandLineOptions.ReplaceInFiles(args);
                if (!rename.Execute())
                {
                    Console.WriteLine(rename.ErrorMessage);
                }
            }
            else if (args.HasArg("ExecuteCommand"))
            {
                var rename = new CommandLineOptions.ExecuteCommand(args);
                if (!rename.Execute())
                {
                    Console.WriteLine(rename.ErrorMessage);
                }
            }
            else if (args.HasArg("AppendRubric"))
            {
                var rename = new CommandLineOptions.AppendRubric(args);
                if (!rename.Execute())
                {
                    Console.WriteLine(rename.ErrorMessage);
                }
            }
            Environment.Exit(0);
        }
    }
}
