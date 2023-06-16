using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [InitializeAfterPlugins]
    public static class BackgroundWorker
    {
        static BackgroundWorker()
        {
            queue = new ConcurrentQueue<Action>();

            EditorApplication.delayCall += delegate
            {
                ClearProgress();

                EditorApplication.update += DisplayProgress;
                new Thread(Work) { Name = "Background Worker" }.Start();
            };
        }

        private static readonly object @lock = new object();
        private static bool clearProgress;

        private static int progressId = -1;

        private static readonly ConcurrentQueue<Action> queue;

        public static event Action tasks
        {
            add
            {
                Schedule(value);
            }
            remove { }
        }

        public static string progressLabel { get; private set; }
        public static float progressProportion { get; private set; }
        public static bool hasProgress => progressLabel != null;

        public static void Schedule(Action action)
        {
            queue.Enqueue(action);
        }

        private static void Work()
        {
            while (true)
            {
                if (queue.TryDequeue(out var task))
                {
                    var remaining = queue.Count + 1;

                    ReportProgress($"{remaining} task{(queue.Count > 1 ? "s" : "")} remaining...", 0);

                    try
                    {
                        task();
                    }
                    catch (Exception ex)
                    {
                        EditorApplication.delayCall += () => Debug.LogException(ex);
                    }
                    finally
                    {
                        ClearProgress();
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static void ReportProgress(string title, float progress)
        {
            lock (@lock)
            {
                progressLabel = title;
                progressProportion = progress;
            }
        }

        public static void ClearProgress()
        {
            lock (@lock)
            {
                clearProgress = true;
                progressLabel = null;
                progressProportion = 0;
            }
        }

        private static void DisplayProgress()
        {
            lock (@lock)
            {
                if (clearProgress)
                {
                    try
                    {
                        if (progressId != -1)
                        {
                            progressId = Progress.Remove(progressId);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new UnityEditorInternalException(ex);
                    }


                    clearProgress = false;
                }

                if (progressLabel != null)
                {
                    try
                    {
                        if (progressId == -1)
                        {
                            progressId = (int)Progress.Start("Ludiq Background Worker",
                                progressLabel, Progress.Options.None, -1);
                        }
                        else
                        {
                            Progress.Report(progressId, progressProportion, progressLabel);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new UnityEditorInternalException(ex);
                    }
                }
            }
        }
    }
}
