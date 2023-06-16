using System;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Common;
using Codice.Client.Common.Connection;
using Codice.Client.Common.Encryption;
using Codice.Client.Common.EventTracking;
using Codice.Client.Common.FsNodeReaders;
using Codice.Client.Common.FsNodeReaders.Watcher;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.CM.ConfigureHelper;
using Codice.LogWrapper;
using Codice.Utils;
using CodiceApp.EventTracking;
using MacUI;
using PlasticGui;
using PlasticGui.EventTracking;
using PlasticGui.WebApi;
using PlasticPipe.Certificates;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.Configuration;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    internal static class PlasticApp
    {
        internal static bool IsUnitTesting { get; set; }

        internal static void InitializeIfNeeded()
        {
            if (mIsInitialized)
                return;

            mIsInitialized = true;

            ConfigureLogging();

            if (!IsUnitTesting)
                GuiMessage.Initialize(new UnityPlasticGuiMessage());

            RegisterExceptionHandlers();

            InitLocalization();

            if (!IsUnitTesting)
                ThreadWaiter.Initialize(new UnityThreadWaiterBuilder());

            ServicePointConfigurator.ConfigureServicePoint();
            CertificateUi.RegisterHandler(new ChannelCertificateUiImpl());

            SetupFsWatcher();

            EditionManager.Get().DisableCapability(
                EnumEditionCapabilities.Extensions);

            ClientHandlers.Register();

            PlasticGuiConfig.SetConfigFile(
                PlasticGuiConfig.UNITY_GUI_CONFIG_FILE);

            if (!IsUnitTesting)
            {
                sEventSenderScheduler = EventTracking.Configure(
                    (PlasticWebRestApi)PlasticGui.Plastic.WebRestAPI,
                    ApplicationIdentifier.UnityPackage,
                    IdentifyEventPlatform.Get());
            }

            if (sEventSenderScheduler != null)
            {
                sPingEventLoop = new PingEventLoop();
                sPingEventLoop.Start();
                sPingEventLoop.SetUnityVersion(Application.unityVersion);

                CollabPlugin.GetVersion(pluginVersion => sPingEventLoop.SetPluginVersion(pluginVersion));
            }

            PlasticMethodExceptionHandling.InitializeAskCredentialsUi(
                new CredentialsUiImpl());
            ClientEncryptionServiceProvider.SetEncryptionPasswordProvider(
                new MissingEncryptionPasswordPromptHandler());
        }

        internal static void SetWorkspace(WorkspaceInfo wkInfo)
        {
            RegisterApplicationFocusHandlers();
            RegisterAssemblyReloadHandlers();

            if (sEventSenderScheduler == null)
                return;

            sPingEventLoop.SetWorkspace(wkInfo);
            PlasticGui.Plastic.WebRestAPI.SetToken(
                CmConnection.Get().BuildWebApiTokenForCloudEditionDefaultUser());
        }

        internal static void Dispose()
        {
            mIsInitialized = false;

            UnRegisterExceptionHandlers();

            UnRegisterApplicationFocusHandlers();
            UnRegisterAssemblyReloadHandlers();

            if (sEventSenderScheduler != null)
            {
                sPingEventLoop.Stop();
                // Launching and forgetting to avoid a timeout when sending events files and no
                // network connection is available.
                // This will be refactored once a better mechanism to send event is in place
                sEventSenderScheduler.EndAndSendEventsAsync();
            }

            WorkspaceInfo wkInfo = FindWorkspace.InfoForApplicationPath(
                ApplicationDataPath.Get(), PlasticGui.Plastic.API);

            if (wkInfo == null)
                return;

            WorkspaceFsNodeReaderCachesCleaner.CleanWorkspaceFsNodeReader(wkInfo);
        }

        static void InitLocalization()
        {
            string language = null;
            try
            {
                language = ClientConfig.Get().GetLanguage();
            }
            catch
            {
                language = string.Empty;
            }

            Localization.Init(language);
            PlasticLocalization.SetLanguage(language);
        }

        static void ConfigureLogging()
        {
            try
            {
                string log4netpath = ToolConfig.GetUnityPlasticLogConfigFile();

                if (!File.Exists(log4netpath))
                    WriteLogConfiguration.For(log4netpath);

                XmlConfigurator.Configure(new FileInfo(log4netpath));
            }
            catch
            {
                //it failed configuring the logging info; nothing to do.
            }
        }

        static void RegisterExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            Application.logMessageReceivedThreaded += HandleLog;
        }

        static void RegisterApplicationFocusHandlers()
        {
            EditorWindowFocus.OnApplicationActivated += EnableFsWatcher;

            EditorWindowFocus.OnApplicationDeactivated += DisableFsWatcher;
        }

        static void UnRegisterExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;

            Application.logMessageReceivedThreaded -= HandleLog;
        }

        static void UnRegisterApplicationFocusHandlers()
        {
            EditorWindowFocus.OnApplicationActivated -= EnableFsWatcher;

            EditorWindowFocus.OnApplicationDeactivated -= DisableFsWatcher;
        }

        static void RegisterAssemblyReloadHandlers()
        {
            AssemblyReloadEvents.beforeAssemblyReload += DisableFsWatcher;
            AssemblyReloadEvents.afterAssemblyReload += EnableFsWatcher;
        }

        static void UnRegisterAssemblyReloadHandlers()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= DisableFsWatcher;
            AssemblyReloadEvents.afterAssemblyReload -= EnableFsWatcher;
        }

        static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;

            if (IsExitGUIException(ex) ||
                !IsPlasticStackTrace(ex.StackTrace))
                throw ex;

            GUIActionRunner.RunGUIAction(delegate {
                ExceptionsHandler.HandleException("HandleUnhandledException", ex);
            });
        }

        static void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Exception)
                return;

            if (!IsPlasticStackTrace(stackTrace))
                return;

            GUIActionRunner.RunGUIAction(delegate {
                mLog.ErrorFormat("[HandleLog] Unexpected error: {0}", logString);
                mLog.DebugFormat("Stack trace: {0}", stackTrace);

                string message = logString;
                if (ExceptionsHandler.DumpStackTrace())
                    message += Environment.NewLine + stackTrace;

                GuiMessage.ShowError(message);
            });
        }

        static void EnableFsWatcher()
        {
            MonoFileSystemWatcher.IsEnabled = true;
        }

        static void DisableFsWatcher()
        {
            MonoFileSystemWatcher.IsEnabled = false;
        }

        static void SetupFsWatcher()
        {
            if (!PlatformIdentifier.IsMac())
                return;

            WorkspaceWatcherFsNodeReadersCache.Get().SetMacFsWatcherBuilder(
                new MacFsWatcherBuilder());
        }

        static bool IsPlasticStackTrace(string stackTrace)
        {
            if (stackTrace == null)
                return false;

            string[] namespaces = new[] {
                "Codice.",
                "GluonGui.",
                "PlasticGui."
            };

            return namespaces.Any(stackTrace.Contains);
        }

        static bool IsExitGUIException(Exception ex)
        {
            return ex is ExitGUIException;
        }

        static bool mIsInitialized;

        static EventSenderScheduler sEventSenderScheduler;
        static PingEventLoop sPingEventLoop;

        static readonly ILog mLog = LogManager.GetLogger("PlasticApp");
    }
}