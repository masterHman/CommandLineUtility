using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLineUtility.Services;
using System.Configuration;

namespace CommandLineUtility.CommandLineOptions
{
    public class ReplaceInFiles
    {
        private Dictionary<string, string> _requiredParameters;
        private Dictionary<string, string> _parameters;
        private List<string> ExcludedFileExtentions { get; set; }
        private List<string> ExcludedFolders { get; set; }
        private string[] _args;

        public string ErrorMessage { get; set; }

        public ReplaceInFiles(string[] args)
        {
            _args = args;
            ExcludedFileExtentions = ConfigurationManager.AppSettings["ReplaceInFiles_ExcludedFileExtentions"].Split(',').ToList();
            ExcludedFolders = ConfigurationManager.AppSettings["ReplaceInFiles_ExcludedFolders"].Split(',').ToList();

            _requiredParameters = new Dictionary<string, string>();
            _requiredParameters.Add("SearchDir", "");
            _requiredParameters.Add("SearchValue", "");
            _requiredParameters.Add("ReplaceValue", "");

        }

        public bool Execute()
        {
            try
            {
                if (_args.HasArgsWithValues(_requiredParameters.Keys.ToArray()))
                {
                    _parameters = _args.GetArguments();

                    var root = new DirectoryInfo(_args.GetArg("SearchDir"));
                    if (root.Exists)
                    {
                        root = new DirectoryInfo(_parameters["SearchDir"]);
                        WalkDirectory(_parameters["SearchValue"], _parameters["ReplaceValue"], root);
                    }
                    else
                    {
                        ErrorMessage = string.Format("Directory not found: {0}", _args.GetArg("SearchDir"));
                        return false;
                    }
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

        private void WalkDirectory(string searchValue, string replaceValue, DirectoryInfo root)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.*").Where(fi => !ExcludedFileExtentions.Contains(fi.Extension, StringComparer.OrdinalIgnoreCase)).ToArray();
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().
                    string text = File.ReadAllText(fi.FullName);
                    if (text.Contains(searchValue) && !fi.Attributes.HasFlag(FileAttributes.Hidden) && !fi.Attributes.HasFlag(FileAttributes.ReadOnly))
                    {
                        text = text.Replace(searchValue, replaceValue);
                        File.WriteAllText(fi.FullName, text);

                    }
                    Console.WriteLine(fi.FullName);
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories().Where(dir => !ExcludedFolders.Contains(dir.Name, StringComparer.OrdinalIgnoreCase)).ToArray();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectory(searchValue, replaceValue, dirInfo);
                }
            }
        }
    }
}
