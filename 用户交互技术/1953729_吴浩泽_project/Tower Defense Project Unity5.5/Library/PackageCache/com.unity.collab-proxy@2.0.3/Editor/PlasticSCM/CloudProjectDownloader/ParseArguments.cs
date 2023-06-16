using System.Collections.Generic;

namespace Unity.PlasticSCM.Editor.ProjectDownloader
{
    internal static class ParseArguments
    {
        internal static string CloudProject(Dictionary<string, string> args)
        {
            string data;

            if (!args.TryGetValue(CLOUD_PROJECT, out data))
                return null;

            return data;
        }

        internal static string CloudOrganization(Dictionary<string, string> args)
        {
            string data;

            if (!args.TryGetValue(CLOUD_ORGANIZATION, out data))
                return null;

            return GetOrganizationNameFromData(data);
        }

        internal static string ProjectPath(Dictionary<string, string> args)
        {
            string data;

            if (!args.TryGetValue(CREATE_PROJECT, out data))
                return null;

            return data;
        }

        static string GetOrganizationNameFromData(string data)
        {
            // data is in format: 151d73c7-38cb-4eec-b11e-34764e707226-danipen-unity
            int guidLenght = 36;

            if (data.Length < guidLenght + 1)
                return null;

            return data.Substring(guidLenght + 1);
        }

        const string CLOUD_PROJECT = "-cloudProject";
        const string CLOUD_ORGANIZATION = "-cloudOrganization";
        const string CREATE_PROJECT = "-createProject";

    }
}
