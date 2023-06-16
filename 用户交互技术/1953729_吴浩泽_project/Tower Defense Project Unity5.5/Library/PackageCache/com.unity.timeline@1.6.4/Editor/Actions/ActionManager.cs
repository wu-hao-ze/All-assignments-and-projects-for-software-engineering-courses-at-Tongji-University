using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline.Actions
{
    static class ActionManager
    {
        static bool s_ShowActionTriggeredByShortcut = false;

        public static readonly IReadOnlyList<TimelineAction> TimelineActions = InstantiateClassesOfType<TimelineAction>();
        public static readonly IReadOnlyList<ClipAction> ClipActions = InstantiateClassesOfType<ClipAction>();
        public static readonly IReadOnlyList<TrackAction> TrackActions = InstantiateClassesOfType<TrackAction>();
        public static readonly IReadOnlyList<MarkerAction> MarkerActions = InstantiateClassesOfType<MarkerAction>();

        public static readonly IReadOnlyList<TimelineAction> TimelineActionsWithShortcuts = ActionsWithShortCuts(TimelineActions);
        public static readonly IReadOnlyList<ClipAction> ClipActionsWithShortcuts = ActionsWithShortCuts(ClipActions);
        public static readonly IReadOnlyList<TrackAction> TrackActionsWithShortcuts = ActionsWithShortCuts(TrackActions);
        public static readonly IReadOnlyList<MarkerAction> MarkerActionsWithShortcuts = ActionsWithShortCuts(MarkerActions);

        public static readonly HashSet<Type> ActionsWithAutoUndo = TypesWithAttribute<ApplyDefaultUndoAttribute>();

        public static TU GetCachedAction<T, TU>(this IReadOnlyList<TU> list) where T : TU
        {
            return list.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        public static void GetMenuEntries(IReadOnlyList<TimelineAction> actions, Vector2? mousePos, List<MenuActionItem> menuItems)
        {
            var globalContext = TimelineEditor.CurrentContext(mousePos);
            foreach (var action in actions)
            {
                try
                {
                    BuildMenu(action, globalContext, menuItems);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static void GetMenuEntries(IReadOnlyList<TrackAction> actions, List<MenuActionItem> menuItems)
        {
            var tracks = SelectionManager.SelectedTracks();
            if (!tracks.Any())
                return;

            foreach (var action in actions)
            {
                try
                {
                    BuildMenu(action, tracks, menuItems);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static void GetMenuEntries(IReadOnlyList<ClipAction> actions, List<MenuActionItem> menuItems)
        {
            var clips = SelectionManager.SelectedClips();
            bool any = clips.Any();
            if (!clips.Any())
                return;

            foreach (var action in actions)
            {
                try
                {
                    if (action is EditSubTimeline editSubTimelineAction)
                        editSubTimelineAction.AddMenuItem(menuItems);
                    else if (any)
                        BuildMenu(action, clips, menuItems);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static void GetMenuEntries(IReadOnlyList<MarkerAction> actions, List<MenuActionItem> menuItems)
        {
            var markers = SelectionManager.SelectedMarkers();
            if (!markers.Any())
                return;

            foreach (var action in actions)
            {
                try
                {
                    BuildMenu(action, markers, menuItems);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        static void BuildMenu(TimelineAction action, ActionContext context, List<MenuActionItem> menuItems)
        {
            BuildMenu(action, action.Validate(context), () => ExecuteTimelineAction(action, context), menuItems);
        }

        static void BuildMenu(TrackAction action, IEnumerable<TrackAsset> tracks, List<MenuActionItem> menuItems)
        {
            BuildMenu(action, action.Validate(tracks), () => ExecuteTrackAction(action, tracks), menuItems);
        }

        static void BuildMenu(ClipAction action, IEnumerable<TimelineClip> clips, List<MenuActionItem> menuItems)
        {
            BuildMenu(action, action.Validate(clips), () => ExecuteClipAction(action, clips), menuItems);
        }

        static void BuildMenu(MarkerAction action, IEnumerable<IMarker> markers, List<MenuActionItem> menuItems)
        {
            BuildMenu(action, action.Validate(markers), () => ExecuteMarkerAction(action, markers), menuItems);
        }

        static void BuildMenu(IAction action, ActionValidity validity, GenericMenu.MenuFunction executeFunction, List<MenuActionItem> menuItems)
        {
            var menuAttribute = action.GetType().GetCustomAttribute<MenuEntryAttribute>(false);
            if (menuAttribute == null)
                return;

            if (validity == ActionValidity.NotApplicable)
                return;

            var menuActionItem = new MenuActionItem
            {
                state = validity,
                entryName = action.GetMenuEntryName(),
                priority = menuAttribute.priority,
                category = menuAttribute.subMenuPath,
                isActiveInMode = action.IsActionActiveInMode(TimelineWindow.instance.currentMode.mode),
                shortCut = action.GetShortcut(),
                callback = executeFunction,
                isChecked = action.IsChecked()
            };
            menuItems.Add(menuActionItem);
        }

        internal static void BuildMenu(GenericMenu menu, List<MenuActionItem> items)
        {
            // sorted the outer menu by priority, then sort the innermenu by priority
            var sortedItems =
                items.GroupBy(x => string.IsNullOrEmpty(x.category) ? x.entryName : x.category).OrderBy(x => x.Min(y => y.priority)).SelectMany(x => x.OrderBy(z => z.priority));

            int lastPriority = Int32.MinValue;
            string lastCategory = string.Empty;

            foreach (var s in sortedItems)
            {
                if (s.state == ActionValidity.NotApplicable)
                    continue;

                var priority = s.priority;
                if (lastPriority != int.MinValue && priority / MenuPriority.separatorAt > lastPriority / MenuPriority.separatorAt)
                {
                    string path = string.Empty;
                    if (lastCategory == s.category)
                        path = s.category;
                    menu.AddSeparator(path);
                }

                lastPriority = priority;
                lastCategory = s.category;

                string entry = s.category + s.entryName;
                if (!string.IsNullOrEmpty(s.shortCut))
                    entry += " " + s.shortCut;

                if (s.state == ActionValidity.Valid && s.isActiveInMode)
                    menu.AddItem(new GUIContent(entry), s.isChecked, s.callback);
                else
                    menu.AddDisabledItem(new GUIContent(entry), s.isChecked);
            }
        }

        public static bool HandleShortcut(Event evt)
        {
            if (EditorGUI.IsEditingTextField())
                return false;

            return HandleShortcut(evt, TimelineActionsWithShortcuts, (x) => ExecuteTimelineAction(x, TimelineEditor.CurrentContext())) ||
                HandleShortcut(evt, ClipActionsWithShortcuts, (x => ExecuteClipAction(x, SelectionManager.SelectedClips()))) ||
                HandleShortcut(evt, TrackActionsWithShortcuts, (x => ExecuteTrackAction(x, SelectionManager.SelectedTracks()))) ||
                HandleShortcut(evt, MarkerActionsWithShortcuts, (x => ExecuteMarkerAction(x, SelectionManager.SelectedMarkers())));
        }

        public static bool HandleShortcut<T>(Event evt, IReadOnlyList<T> actions, Func<T, bool> invoke) where T : class, IAction
        {
            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                var attr = action.GetType().GetCustomAttributes(typeof(ShortcutAttribute), true);

                foreach (ShortcutAttribute shortcut in attr)
                {
                    if (shortcut.MatchesEvent(evt))
                    {
                        if (s_ShowActionTriggeredByShortcut)
                            Debug.Log(action.GetType().Name);

                        if (!action.IsActionActiveInMode(TimelineWindow.instance.currentMode.mode))
                            continue;

                        if (invoke(action))
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool ExecuteTimelineAction(TimelineAction timelineAction, ActionContext context)
        {
            if (timelineAction.Validate(context) == ActionValidity.Valid)
            {
                if (timelineAction.HasAutoUndo())
                    UndoExtensions.RegisterContext(context, timelineAction.GetUndoName());
                return timelineAction.Execute(context);
            }
            return false;
        }

        public static bool ExecuteTrackAction(TrackAction trackAction, IEnumerable<TrackAsset> tracks)
        {
            if (tracks != null && tracks.Any() && trackAction.Validate(tracks) == ActionValidity.Valid)
            {
                if (trackAction.HasAutoUndo())
                    UndoExtensions.RegisterTracks(tracks, trackAction.GetUndoName());
                return trackAction.Execute(tracks);
            }
            return false;
        }

        public static bool ExecuteClipAction(ClipAction clipAction, IEnumerable<TimelineClip> clips)
        {
            if (clips != null && clips.Any() && clipAction.Validate(clips) == ActionValidity.Valid)
            {
                if (clipAction.HasAutoUndo())
                    UndoExtensions.RegisterClips(clips, clipAction.GetUndoName());
                return clipAction.Execute(clips);
            }
            return false;
        }

        public static bool ExecuteMarkerAction(MarkerAction markerAction, IEnumerable<IMarker> markers)
        {
            if (markers != null && markers.Any() && markerAction.Validate(markers) == ActionValidity.Valid)
            {
                if (markerAction.HasAutoUndo())
                    UndoExtensions.RegisterMarkers(markers, markerAction.GetUndoName());
                return markerAction.Execute(markers);
            }
            return false;
        }

        static List<T> InstantiateClassesOfType<T>() where T : class
        {
            var typeCollection = TypeCache.GetTypesDerivedFrom(typeof(T));
            var list = new List<T>(typeCollection.Count);
            for (int i = 0; i < typeCollection.Count; i++)
            {
                if (typeCollection[i].IsAbstract || typeCollection[i].IsGenericType)
                    continue;

                if (typeCollection[i].GetConstructor(Type.EmptyTypes) == null)
                {
                    Debug.LogWarning($"{typeCollection[i].FullName} requires a default constructor to be automatically instantiated by Timeline");
                    continue;
                }

                list.Add((T)Activator.CreateInstance(typeCollection[i]));
            }
            return list;
        }

        static List<T> ActionsWithShortCuts<T>(IReadOnlyList<T> list)
        {
            return list.Where(x => x.GetType().GetCustomAttributes(typeof(ShortcutAttribute), true).Length > 0).ToList();
        }

        static HashSet<System.Type> TypesWithAttribute<T>() where T : Attribute
        {
            var hashSet = new HashSet<System.Type>();
            var typeCollection = TypeCache.GetTypesWithAttribute(typeof(T));
            for (int i = 0; i < typeCollection.Count; i++)
            {
                hashSet.Add(typeCollection[i]);
            }

            return hashSet;
        }
    }
}
