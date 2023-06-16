using System;
using System.Threading;

using UnityEditor;

using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Common.Connection;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.LogWrapper;
using PlasticPipe;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    internal class PlasticConnectionMonitor :
        HandleCredsAliasAndServerCert.IHostUnreachableExceptionListener
    {
        internal bool IsTryingReconnection { get{ return mIsTryingReconnection; } }
        internal bool IsConnected { get{ return mIsConnected; } }

        internal void CheckConnection()
        {
            mIsTryingReconnection = true;
            mResetEvent.Set();
        }

        internal void SetAsConnected()
        {
            mIsConnected = true;
        }

        internal void Stop()
        {
            mIsMonitoringServerConnection = false;
            mResetEvent.Set();
        }

        internal void SetRepositorySpecForEventTracking(RepositorySpec repSpec)
        {
            mRepSpecForEventTracking = repSpec;
        }

        internal void OnConnectionError(Exception ex, string server)
        {
            if (!mIsConnected)
            {
                mLog.WarnFormat("A network exception happened while the plugin was offline!");
                ExceptionsHandler.LogException("PlasticConnectionMonitor", ex);
                return;
            }

            mLog.Debug("A network exception will cause the Plugin to go offline");
            ExceptionsHandler.LogException("PlasticConnectionMonitor", ex);

            OnConnectionLost(server);
        }

        void HandleCredsAliasAndServerCert.IHostUnreachableExceptionListener.OnHostUnreachableException(
            Exception ex,
            PlasticServer plasticServer)
        {
            OnConnectionError(ex, plasticServer.OriginalUrl);
        }

        void StartMonitoring(string server)
        {
            mIsMonitoringServerConnection = true;

            Thread thread = new Thread(MonitorServerConnection);
            thread.IsBackground = true;
            thread.Start(server);
        }

        void MonitorServerConnection(object obj)
        {
            string server = (string)obj;

            while (true)
            {
                if (!mIsMonitoringServerConnection)
                    break;

                try
                {
                    bool isConnected;

                    mResetEvent.Reset();

                    isConnected = HasConnectionToServer(server);

                    mIsTryingReconnection = false;

                    if (isConnected)
                    {
                        OnConnectionRestored();
                        break;
                    }

                    EditorDispatcher.Dispatch(() =>
                    {
                        PlasticWindow window = GetPlasticWindowIfOpened();

                        if (window != null)
                            window.Repaint();
                    });

                    mResetEvent.WaitOne(CONNECTION_POLL_TIME_MS);
                }
                catch (Exception ex)
                {
                    mLog.Error("Error checking network connectivity", ex);
                    mLog.DebugFormat("Stacktrace: {0}", ex.StackTrace);
                }
            }
        }

        void OnConnectionLost(string server)
        {
            TrackConnectionLostEvent(mRepSpecForEventTracking);

            mIsConnected = false;

            EditorDispatcher.Dispatch(() =>
            {
                PlasticPlugin.Disable();

                StartMonitoring(server);

                PlasticWindow window = GetPlasticWindowIfOpened();

                if (window != null)
                    window.Repaint();
            });
        }

        void OnConnectionRestored()
        {
            TrackConnectionRestoredEvent(mRepSpecForEventTracking);

            mIsConnected = true;

            EditorDispatcher.Dispatch(() =>
            {
                PlasticPlugin.Enable();

                PlasticWindow window = GetPlasticWindowIfOpened();

                if (window != null)
                    window.RefreshWorkspaceUI();
            });
        }

        static bool HasConnectionToServer(string server)
        {
            try
            {
                mLog.DebugFormat("Checking connection to {0}...", server);

                return PlasticGui.Plastic.API.CheckServerConnection(server);
            }
            catch (Exception ex)
            {
                mLog.DebugFormat("Checking connection to {0} failed: {1}",
                    server,
                    ex.Message);
                return false;
            }
        }

        static void TrackConnectionLostEvent(RepositorySpec repSpec)
        {
            if (repSpec == null)
                return;

            TrackFeatureUseEvent.For(
                repSpec,
                TrackFeatureUseEvent.Features.UnityPackage.DisableAutomatically);
        }

        static void TrackConnectionRestoredEvent(RepositorySpec repSpec)
        {
            if (repSpec == null)
                return;

            TrackFeatureUseEvent.For(
                repSpec,
                TrackFeatureUseEvent.Features.UnityPackage.EnableAutomatically);
        }

        static PlasticWindow GetPlasticWindowIfOpened()
        {
            if (!EditorWindow.HasOpenInstances<PlasticWindow>())
                return null;

            return EditorWindow.GetWindow<PlasticWindow>(null, false);
        }

        RepositorySpec mRepSpecForEventTracking;

        volatile bool mIsMonitoringServerConnection;
        volatile bool mIsTryingReconnection;
        volatile bool mIsConnected = true;

        ManualResetEvent mResetEvent = new ManualResetEvent(false);

        const int CONNECTION_POLL_TIME_MS = 30000;

        static readonly ILog mLog = LogManager.GetLogger("PlasticConnectionMonitor");
    }
}
