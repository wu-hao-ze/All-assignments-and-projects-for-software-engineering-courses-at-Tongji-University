using System.Diagnostics;
using System.IO;
using System.Net;

using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.Utils;
using PlasticGui;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WebApi.Responses;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI.UIElements;
using Unity.PlasticSCM.Editor.WebApi;

namespace Unity.PlasticSCM.Editor.Views.Welcome
{
    class DownloadAndInstallOperation
    {
        internal interface INotify
        {
            void InstallationStarted();
            void InstallationFinished();
        }

        internal static void Run(
            Edition plasticEdition,
            string installerDestinationPath,
            ProgressControlsForDialogs progressControls,
            INotify notify)
        {
            ((IProgressControls)progressControls).ShowProgress(
                PlasticLocalization.GetString(PlasticLocalization.Name.DownloadingProgress));

            NewVersionResponse plasticVersion = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    plasticVersion = WebRestApiClient.PlasticScm.
                        GetLastVersion(plasticEdition);

                    if (plasticVersion == null)
                        return;

                    string installerUrl = GetInstallerUrl(
                        plasticVersion.Version,
                        plasticEdition == Edition.Cloud);

                    DownloadInstaller(
                        installerUrl,
                        installerDestinationPath,
                        progressControls);

                    if (!PlatformIdentifier.IsMac())
                        return;

                    installerDestinationPath = UnZipMacOsPackage(
                        installerDestinationPath);
                },
                /*afterOperationDelegate*/ delegate
                {
                    ((IProgressControls)progressControls).HideProgress();

                    if (waiter.Exception != null)
                    {
                        ((IProgressControls)progressControls).ShowError(
                            waiter.Exception.Message);
                        return;
                    }

                    if (plasticVersion == null)
                    {
                        ((IProgressControls)progressControls).ShowError(
                            PlasticLocalization.GetString(PlasticLocalization.Name.ConnectingError));
                        return;
                    }

                    if (!File.Exists(installerDestinationPath))
                        return;

                    RunInstaller(
                        installerDestinationPath,
                        progressControls,
                        notify);
                });
        }

        static void RunInstaller(
            string installerPath,
            ProgressControlsForDialogs progressControls,
            INotify notify)
        {
            progressControls.ProgressData.ProgressPercent = -1;

            ((IProgressControls)progressControls).ShowProgress(
                PlasticLocalization.GetString(PlasticLocalization.Name.InstallingProgress));

            notify.InstallationStarted();

            MacOSConfigWorkaround configWorkaround = new MacOSConfigWorkaround();

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    configWorkaround.CreateClientConfigIfNeeded();

                    Process installerProcess =
                        LaunchInstaller.ForPlatform(installerPath);

                    if (installerProcess != null)
                        installerProcess.WaitForExit();

                    configWorkaround.DeleteClientConfigIfNeeded();
                },
                /*afterOperationDelegate*/ delegate
                {
                    notify.InstallationFinished();
                    ((IProgressControls)progressControls).HideProgress();

                    if (waiter.Exception != null)
                    {
                        ((IProgressControls)progressControls).ShowError(
                            waiter.Exception.Message);
                        return;
                    }

                    File.Delete(installerPath);
                });
        }

        static void DownloadInstaller(
            string url,
            string destinationPath,
            ProgressControlsForDialogs progressControls)
        {
            int bytesProcessed = 0;

            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;

            try
            {
                WebRequest request = WebRequest.Create(url);
                response = request.GetResponse();

                long totalBytes = response.ContentLength;

                if (File.Exists(destinationPath) &&
                    new FileInfo(destinationPath).Length == totalBytes)
                {
                    UnityEngine.Debug.LogFormat(
                        PlasticLocalization.GetString(PlasticLocalization.Name.SkippingDownloadFileExists),
                        destinationPath);
                    return;
                }

                remoteStream = response.GetResponseStream();

                localStream = File.Create(destinationPath);

                byte[] buffer = new byte[100 * 1024];
                int bytesRead;

                do
                {
                    bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
                    localStream.Write(buffer, 0, bytesRead);
                    bytesProcessed += bytesRead;

                    progressControls.ProgressData.ProgressPercent =
                        GetProgressBarPercent.ForTransfer(
                            bytesProcessed,
                            totalBytes) / 100f;
                } while (bytesRead > 0);
            }
            finally
            {
                if (response != null)
                    response.Close();

                if (remoteStream != null)
                    remoteStream.Close();

                if (localStream != null)
                    localStream.Close();
            }
        }

        static string UnZipMacOsPackage(
            string zipInstallerPath)
        {
            try
            {
                string pkgInstallerPath = zipInstallerPath.Substring(
                    0, zipInstallerPath.Length - ".zip".Length);

                string unzipCommand = string.Format(
                    "unzip -p \"{0}\" > \"{1}\"",
                    zipInstallerPath, pkgInstallerPath);

                unzipCommand = unzipCommand.Replace("\"", "\"\"");

                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = "/bin/bash";
                processStartInfo.Arguments = string.Format("-c \"{0}\"", unzipCommand);
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;

                Process process = Process.Start(processStartInfo);
                process.WaitForExit();

                return pkgInstallerPath;
            }
            finally
            {
                File.Delete(zipInstallerPath);
            }
        }

        static string GetInstallerUrl(
            string version,
            bool isCloudEdition)
        {
            string edition = isCloudEdition ?
                "cloudedition" : "full";

            string platform = PlatformIdentifier.IsMac() ?
                "macosx" : "windows";

            return string.Format(
                @"https://www.plasticscm.com/download/downloadinstaller/{0}/plasticscm/{1}/{2}",
                version,
                platform,
                edition);
        }
    }
}
