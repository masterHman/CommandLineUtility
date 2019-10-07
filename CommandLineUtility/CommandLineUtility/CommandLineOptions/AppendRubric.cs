using CommandLineUtility.Services;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineUtility.CommandLineOptions
{
    public class AppendRubric
    {
        private Dictionary<string, string> _requiredParameters;
        private Dictionary<string, string> _parameters;
        private string[] _args;
        public string RubricPath { get; private set; }
        public string ResultPath { get; private set; }

        public string ErrorMessage { get; set; }

        public AppendRubric(string[] args)
        {
            _args = args;

            _requiredParameters = new Dictionary<string, string>();
            _requiredParameters.Add("dir", "");
            _requiredParameters.Add("rubric", "");
            _requiredParameters.Add("graded", "");
        }

        public bool Execute()
        {

            try
            {
                if (_args.HasArgsWithValues(_requiredParameters.Keys.ToArray()))
                {
                    _parameters = _args.GetArguments();
                    var workingDir = _parameters["dir"];
                    RubricPath = _parameters["rubric"];
                    ResultPath = _parameters["graded"];


                    var root = new DirectoryInfo(workingDir);
                    if (root.Exists)
                    {
                        WalkDirectory(root);
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

        private void WalkDirectory(DirectoryInfo root)
        {
            FileInfo[] submissions = null;
            DirectoryInfo[] subDirs = null;

            try
            {
                submissions = root.GetFiles("*.pdf");
                foreach (FileInfo submission in submissions)
                {
                    Console.WriteLine(submission.FullName);
                    MergePDFs(root.Name, submission);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            subDirs = root.GetDirectories();

            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                WalkDirectory(dirInfo);
            }
        }

        public bool MergePDFs(string dirName, FileInfo submission)
        {
            bool merged = true;
            var outDir = new DirectoryInfo(Path.Combine(ResultPath, dirName));
            if (outDir.Exists == false)
            {
                outDir.Create();
            }
            var outFile = Path.Combine(ResultPath, dirName, submission.Name);
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFile));
            PdfMerger merger = new PdfMerger(pdf);

            //Add pages from the first document
            PdfDocument firstSourcePdf = new PdfDocument(new PdfReader(submission));
            merger.Merge(firstSourcePdf, 1, firstSourcePdf.GetNumberOfPages());

            //Add pages from the second pdf document
            PdfDocument secondSourcePdf = new PdfDocument(new PdfReader(RubricPath));
            merger.Merge(secondSourcePdf, 1, secondSourcePdf.GetNumberOfPages());

            firstSourcePdf.Close();
            secondSourcePdf.Close();
            pdf.Close();
            return merged;
        }
    }
}
