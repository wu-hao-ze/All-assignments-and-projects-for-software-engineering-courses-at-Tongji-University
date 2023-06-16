using System.Collections;

using Codice.CM.Common;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace
{
    internal static class ValidRepositoryName
    {
        internal static string Get(string repositoryName, IList repositories)
        {
            string validRepositoryName = GetValidRepositoryName(repositoryName);
            string result = validRepositoryName;

            int i = 2;

            while (RepositoryExists(result, repositories))
            {
                result = validRepositoryName + "_" + i.ToString();
                i++;
            }

            return result;
        }

        static bool RepositoryExists(string repositoryName, IList repositories)
        {
            if (repositories == null)
                return false;

            foreach (RepositoryInfo repInfo in repositories)
            {
                if (repInfo.Name.Equals(repositoryName))
                    return true;
            }

            return false;
        }

        static string GetValidRepositoryName(string newRepository)
        {
            string result = newRepository.Replace(SUBMODULE_SEPARATOR, '-');
            result = result.Replace(PIPE_CHARACTER, '-');
            return result;
        }

        const char SUBMODULE_SEPARATOR = '/';
        const char PIPE_CHARACTER = '|';
    }
}
