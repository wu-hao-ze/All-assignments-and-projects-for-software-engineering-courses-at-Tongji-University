using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using UnityEditor.Build;

namespace Unity.VisualScripting
{
    public static class DocumentationGenerator
    {
        public static string GenerateDocumentation(string projectPath)
        {
            PathUtility.CreateDirectoryIfNeeded(BoltCore.Paths.assemblyDocumentations);

            var projectName = Path.GetFileNameWithoutExtension(projectPath);

            if (!File.Exists(projectPath))
            {
                throw new FileNotFoundException($"Project file not found: '{projectPath}'.");
            }

            var projectBuilderPath = Paths.projectBuilder;

            if (!File.Exists(projectBuilderPath))
            {
                throw new FileNotFoundException($"Project builder not found: '{projectBuilderPath}'.\nYou can download the latest MSBuild from: {Paths.MsBuildDownloadLink}");
            }

            using (var process = new Process())
            {
                var projectXml = XDocument.Load(projectPath);
                var projectRootNamespace = projectXml.Root.GetDefaultNamespace();
                var assemblyName = projectXml.Descendants(projectRootNamespace + "AssemblyName").Single().Value;
                var documentationPath = Path.Combine(BoltCore.Paths.assemblyDocumentations, assemblyName + ".xml");

                process.StartInfo = new ProcessStartInfo();
                process.StartInfo.FileName = projectBuilderPath;

                process.StartInfo.Arguments =
                    "/p:Configuration=Debug " +
                    "/p:GenerateDocumentation=true " +
                    "/p:WarningLevel=0 " +
                    $"/p:DocumentationFile=\"{documentationPath}\" " +
                    $"\"{projectPath}\"";

                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;

                var timeout = 20000;

                var output = new StringBuilder();
                var error = new StringBuilder();

                using (var outputWaitHandle = new AutoResetEvent(false))
                using (var errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (process.WaitForExit(timeout) &&
                        outputWaitHandle.WaitOne(timeout) &&
                        errorWaitHandle.WaitOne(timeout))
                    {
                        if (process.ExitCode != 0)
                        {
                            throw new BuildFailedException($"Failed to build project '{projectName}':\n{process.StartInfo.Arguments}\n{error}\n{output}");
                        }

                        XmlDocumentation.ClearCache();

                        return output.ToString();
                    }
                    else
                    {
                        throw new TimeoutException("Build process timed out.");
                    }
                }
            }
        }
    }
}
