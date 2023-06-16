using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace UnityEditor.Timeline.Actions
{
    /// interface indicating an Action class
    interface IAction { }

    /// extension methods for IActions
    static class ActionExtensions
    {
        const string kActionPostFix = "Action";

        public static string GetUndoName(this IAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var attr = action.GetType().GetCustomAttribute<ApplyDefaultUndoAttribute>(false);
            if (attr != null && !string.IsNullOrWhiteSpace(attr.UndoTitle))
                return attr.UndoTitle;

            return action.GetDisplayName();
        }

        public static string GetMenuEntryName(this IAction action)
        {
            var menuAction = action as IMenuName;
            if (menuAction != null && !string.IsNullOrWhiteSpace(menuAction.menuName))
                return menuAction.menuName;

            var attr = action.GetType().GetCustomAttribute<MenuEntryAttribute>(false);
            if (attr != null && !string.IsNullOrWhiteSpace(attr.name))
                return attr.name;

            return action.GetDisplayName();
        }

        public static string GetDisplayName(this IAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var attr = action.GetType().GetCustomAttribute<DisplayNameAttribute>(false);
            if (attr != null && !string.IsNullOrEmpty(attr.DisplayName))
                return attr.DisplayName;

            var name = action.GetType().Name;
            if (name.EndsWith(kActionPostFix))
                return ObjectNames.NicifyVariableName(name.Substring(0, name.Length - kActionPostFix.Length));

            return ObjectNames.NicifyVariableName(name);
        }

        public static bool HasAutoUndo(this IAction action)
        {
            return action != null && ActionManager.ActionsWithAutoUndo.Contains(action.GetType());
        }

        public static bool IsChecked(this IAction action)
        {
            return (action is IMenuChecked menuAction) && menuAction.isChecked;
        }

        public static bool IsActionActiveInMode(this IAction action, TimelineModes mode)
        {
            var attr = action.GetType().GetCustomAttribute<ActiveInModeAttribute>(true);
            return attr != null && (attr.modes & mode) != 0;
        }

        public static string GetShortcut(this IAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var shortcutAttribute = GetShortcutAttributeForAction(action);
            var shortCut = shortcutAttribute == null ? string.Empty : shortcutAttribute.GetMenuShortcut();
            if (string.IsNullOrWhiteSpace(shortCut))
            {
                //Check if there is a static method with attribute
                var customShortcutMethod = action.GetType().GetMethods().FirstOrDefault(m => m.GetCustomAttribute<TimelineShortcutAttribute>(true) != null);
                if (customShortcutMethod != null)
                {
                    var shortcutId = customShortcutMethod.GetCustomAttribute<TimelineShortcutAttribute>(true).identifier;
                    var shortcut = ShortcutIntegration.instance.directory.FindShortcutEntry(shortcutId);
                    if (shortcut != null && shortcut.combinations.Any())
                        shortCut = KeyCombination.SequenceToMenuString(shortcut.combinations);
                }
            }

            return shortCut;
        }

        static ShortcutAttribute GetShortcutAttributeForAction(this IAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var shortcutAttributes = action.GetType()
                .GetCustomAttributes(typeof(ShortcutAttribute), true)
                .Cast<ShortcutAttribute>();

            foreach (var shortcutAttribute in shortcutAttributes)
            {
                if (shortcutAttribute is ShortcutPlatformOverrideAttribute shortcutOverride)
                {
                    if (shortcutOverride.MatchesCurrentPlatform())
                        return shortcutOverride;
                }
                else
                {
                    return shortcutAttribute;
                }
            }

            return null;
        }
    }
}
