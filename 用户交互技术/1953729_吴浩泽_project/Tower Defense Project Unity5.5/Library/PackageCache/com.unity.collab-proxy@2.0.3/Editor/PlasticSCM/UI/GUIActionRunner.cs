using System;
using System.Threading;

using Codice.LogWrapper;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class GUIActionRunner
    {
        internal delegate void ActionDelegate();

        internal static void RunGUIAction(ActionDelegate action)
        {
            if (EditorDispatcher.IsOnMainThread)
            {
                action();
                return;
            }

            lock (mLock)
            {
                ManualResetEvent syncEvent = new ManualResetEvent(false);

                EditorDispatcher.Dispatch(delegate {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        mLog.ErrorFormat("GUI action failed: {0}", e.Message);
                        mLog.DebugFormat("Stack trace:{0}{1}", Environment.NewLine, e.StackTrace);
                        throw;
                    }
                    finally
                    {
                        syncEvent.Set();
                    }
                });

                syncEvent.WaitOne();
            }
        }

        static object mLock = new object();

        static readonly ILog mLog = LogManager.GetLogger("GUIActionRunner");
    }
}
