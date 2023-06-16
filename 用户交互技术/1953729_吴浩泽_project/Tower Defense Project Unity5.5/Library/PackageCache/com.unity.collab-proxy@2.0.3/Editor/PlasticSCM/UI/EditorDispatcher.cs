using System;
using System.Collections.Generic;
using System.Threading;

using UnityEditor;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class EditorDispatcher
    {
        internal static void Initialize()
        {
            mMainThread = Thread.CurrentThread;
        }

        internal static bool IsOnMainThread
        {
            get { return Thread.CurrentThread == mMainThread; } 
        }

        internal static void Dispatch(Action task)
        {
            lock (mDispatchQueue)
            {
                if (mDispatchQueue.Count == 0)
                    EditorApplication.update += Update;

                mDispatchQueue.Enqueue(task);
            }
        }

        internal static void Update()
        {
            Action[] actions;

            lock (mDispatchQueue)
            {
                if (mDispatchQueue.Count == 0)
                    return;

                actions = mDispatchQueue.ToArray();
                mDispatchQueue.Clear();

                EditorApplication.update -= Update;
            }

            foreach (Action action in actions)
                action();
        }

        static readonly Queue<Action> mDispatchQueue = new Queue<Action>();
        static Thread mMainThread;
    }
}
