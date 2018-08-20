using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineUtility.Services
{
    public static class CommandLineService
    {
        public static string GetArg(this string[] args, string name)
        {
            Dictionary<string, string> kvp = args.GetArguments();

            return kvp.Where(k => k.Key == name).FirstOrDefault().Value;
        }

        public static Dictionary<string, string> GetArguments(this string[] args)
        {
            var kvp = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                var key = arg.Split('=')[0].Replace("-", "");
                var value = arg.IndexOf('=') == -1 ? string.Empty : arg.Substring(arg.IndexOf('=') + 1);

                kvp.Add(key, value);
            }

            return kvp;
        }

        public static bool HasArg(this string[] args, string name)
        {

            return args.GetArguments().Keys.Contains(name);
        }

        public static bool HasArgs(this string[] args, string[] argNames)
        {
            for (int index = 0; index < argNames.Length; index++)
            {
                if (args.GetArguments().Keys.Contains(argNames[index]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool HasArgsWithValues(this string[] args, string[] argNames)
        {
            for (int index = 0; index < argNames.Length; index++)
            {
                if (args.GetArguments().Keys.Contains(argNames[index]) == false)
                {
                    return false;
                }
                if (string.IsNullOrWhiteSpace(args.GetArguments()[argNames[index]]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
