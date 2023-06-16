using System;
using System.Collections.Generic;

namespace Unity.PlasticSCM.Editor.ProjectDownloader
{
    internal class CommandLineArguments
    {
        internal static Dictionary<string, string> Build(string[] args)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            if (args == null)
                return result;
            List<string> trimmedArguments = TrimArgs(args);

            int index = 1;

            while (true)
            {
                if (index > trimmedArguments.Count - 1)
                    break;

                if (IsKeyValueArgumentAtIndex(trimmedArguments, index))
                {
                    result[trimmedArguments[index]] = trimmedArguments[index + 1];
                    index += 2;
                    continue;
                }

                result[trimmedArguments[index]] = null;
                index += 1;
            }

            return result;
        }

        static List<string> TrimArgs(string[] args)
        {
            List<string> trimmedArguments = new List<string>();

            foreach (string argument in args)
                trimmedArguments.Add(argument.Trim());

            return trimmedArguments;
        }

        static bool IsKeyValueArgumentAtIndex(
            List<string> trimmedArguments,
            int index)
        {
            if (index + 1 > trimmedArguments.Count -1)
                return false;

            return !trimmedArguments[index + 1].StartsWith("-");
        }
    }
}
