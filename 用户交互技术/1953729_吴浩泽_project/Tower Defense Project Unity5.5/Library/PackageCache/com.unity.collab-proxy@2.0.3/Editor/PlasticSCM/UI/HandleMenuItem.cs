using PlasticGui;
using System;
using System.Reflection;

using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class HandleMenuItem
    {
        internal static void AddMenuItem(
            string name, 
            int priority,
            Action execute,
            Func<bool> validate)
        {
            AddMenuItem(name, string.Empty, priority, execute, validate);
        }

        internal static void AddMenuItem(
            string name,
            string shortcut,
            int priority,
            Action execute,
            Func<bool> validate)
        {
            MethodInfo InternalAddMenuItem = MenuType.GetMethod(
                "AddMenuItem",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (InternalAddMenuItem == null)
            {
                Debug.LogWarningFormat(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.ErrorAddPlasticSCMMenuItem),
                    name);
                return;
            }

            InternalAddMenuItem.Invoke(
                null, new object[] {
                    name, shortcut, false,
                    priority, execute, validate });
        }

        internal static void RemoveMenuItem(string name)
        {
            MethodInfo InternalRemoveMenuItem = MenuType.GetMethod(
                "RemoveMenuItem",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (InternalRemoveMenuItem == null)
            {
                Debug.LogWarningFormat(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.ErrorRemovePlasticSCMMenuItem),
                    name);
                return;
            }

            InternalRemoveMenuItem.Invoke(
                null, new object[] { name });
        }

        internal static void UpdateAllMenus()
        {
            MethodInfo InternalUpdateAllMenus = EditorUtilityType.GetMethod(
                "Internal_UpdateAllMenus",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (InternalUpdateAllMenus == null)
            {
                Debug.LogWarning(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.ErrorUpdatePlasticSCMMenus));
                return;
            }

            InternalUpdateAllMenus.Invoke(null, null);
        }

        static readonly Type MenuType = typeof(UnityEditor.Menu);
        static readonly Type EditorUtilityType = typeof(UnityEditor.EditorUtility);
    }
}
