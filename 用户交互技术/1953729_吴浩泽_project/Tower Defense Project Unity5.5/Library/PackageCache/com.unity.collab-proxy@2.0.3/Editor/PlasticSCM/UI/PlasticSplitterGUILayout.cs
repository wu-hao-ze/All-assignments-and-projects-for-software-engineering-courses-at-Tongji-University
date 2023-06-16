using System;
using System.Reflection;

using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class PlasticSplitterGUILayout
    {
        internal static void BeginHorizontalSplit(object splitterState)
        {
            InternalBeginHorizontalSplit.Invoke(
                null, new object[] {splitterState, new GUILayoutOption[] { }});
        }

        internal static void EndHorizontalSplit()
        {
            InternalEndHorizontalSplit.Invoke(
                null, new object[] { });
        }

        internal static void BeginVerticalSplit(object splitterState)
        {
            InternalBeginVerticalSplit.Invoke(
                null, new object[] {splitterState, new GUILayoutOption[] { }});
        }

        internal static void EndVerticalSplit()
        {
            InternalEndVerticalSplit.Invoke(
                null, new object[] { });
        }

        internal static object InitSplitterState(
            float[] relativeSizes, int[] minSizes, int[] maxSizes)
        {
            ConstructorInfo ctorInfo = SplitterState.GetConstructor(
                new Type[] {typeof(float[]), typeof(int[]), typeof(int[])});

            return ctorInfo.Invoke(
                new object[] {relativeSizes, minSizes, maxSizes});
        }

        static readonly Type SplitterState =
            typeof(UnityEditor.Editor).Assembly.
                GetType("UnityEditor.SplitterState");
        static readonly Type InternalSplitterGUILayout =
            typeof(UnityEditor.Editor).Assembly.
                GetType("UnityEditor.SplitterGUILayout");

        static readonly MethodInfo InternalBeginHorizontalSplit =
            InternalSplitterGUILayout.GetMethod(
                "BeginHorizontalSplit",
                new Type[] { SplitterState, typeof(GUILayoutOption[]) });
        static readonly MethodInfo InternalEndHorizontalSplit =
            InternalSplitterGUILayout.GetMethod("EndHorizontalSplit");

        static readonly MethodInfo InternalBeginVerticalSplit =
            InternalSplitterGUILayout.GetMethod(
                "BeginVerticalSplit",
                new Type[] { SplitterState, typeof(GUILayoutOption[]) });
        static readonly MethodInfo InternalEndVerticalSplit =
            InternalSplitterGUILayout.GetMethod("EndVerticalSplit");
    }
}
