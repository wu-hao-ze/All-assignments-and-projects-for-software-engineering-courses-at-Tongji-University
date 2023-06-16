using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace UnityEditor.Timeline
{
    static class BindingUtility
    {
        public enum BindingAction
        {
            DoNotBind,
            BindDirectly,
            BindToExistingComponent,
            BindToMissingComponent
        }

        const string k_BindingOperation = "Bind Track";

        public static void Bind(PlayableDirector director, TrackAsset bindTo, Object objectToBind)
        {
            if (director == null || bindTo == null || TimelineWindow.instance == null)
                return;

            if (director.GetGenericBinding(bindTo) == objectToBind)
                return;

            TimelineWindow.instance.state.previewMode = false; // returns all objects to previous state
            TimelineUndo.PushUndo(director, k_BindingOperation);
            director.SetGenericBinding(bindTo, objectToBind);
            TimelineWindow.instance.state.rebuildGraph = true;
        }

        public static void BindWithEditorValidation(PlayableDirector director, TrackAsset bindTo, Object objectToBind)
        {
            TrackEditor trackEditor = CustomTimelineEditorCache.GetTrackEditor(bindTo);
            Object validatedObject = trackEditor.GetBindingFrom_Safe(objectToBind, bindTo);
            Bind(director, bindTo, validatedObject);
        }

        public static void BindWithInteractiveEditorValidation(PlayableDirector director, TrackAsset bindTo, Object objectToBind)
        {
            TrackEditor trackEditor = CustomTimelineEditorCache.GetTrackEditor(bindTo);
            if (trackEditor.SupportsBindingAssign())
                BindWithEditorValidation(director, bindTo, objectToBind);
            else
            {
                Type bindingType = TypeUtility.GetTrackBindingAttribute(bindTo.GetType())?.type;
                BindingAction action = GetBindingAction(bindingType, objectToBind);
                if (action == BindingAction.BindToMissingComponent)
                    InteractiveBindToMissingComponent(director, bindTo, objectToBind, bindingType);
                else
                {
                    var validatedObject = GetBinding(action, objectToBind, bindingType);
                    Bind(director, bindTo, validatedObject);
                }
            }
        }

        public static BindingAction GetBindingAction(Type requiredBindingType, Object objectToBind)
        {
            if (requiredBindingType == null || objectToBind == null)
                return BindingAction.DoNotBind;

            // prevent drag and drop of prefab assets
            if (PrefabUtility.IsPartOfPrefabAsset(objectToBind))
                return BindingAction.DoNotBind;

            if (requiredBindingType.IsInstanceOfType(objectToBind))
                return BindingAction.BindDirectly;

            var draggedGameObject = objectToBind as GameObject;

            if (!typeof(Component).IsAssignableFrom(requiredBindingType) || draggedGameObject == null)
                return BindingAction.DoNotBind;

            if (draggedGameObject.GetComponent(requiredBindingType) == null)
                return BindingAction.BindToMissingComponent;

            return BindingAction.BindToExistingComponent;
        }

        public static Object GetBinding(BindingAction bindingAction, Object objectToBind, Type requiredBindingType)
        {
            if (objectToBind == null) return null;

            switch (bindingAction)
            {
                case BindingAction.BindDirectly:
                {
                    return objectToBind;
                }
                case BindingAction.BindToExistingComponent:
                {
                    var gameObjectBeingDragged = objectToBind as GameObject;
                    Debug.Assert(gameObjectBeingDragged != null, "The object being dragged was detected as being a GameObject");
                    return gameObjectBeingDragged.GetComponent(requiredBindingType);
                }
                case BindingAction.BindToMissingComponent:
                {
                    var gameObjectBeingDragged = objectToBind as GameObject;
                    Debug.Assert(gameObjectBeingDragged != null, "The object being dragged was detected as being a GameObject");
                    return Undo.AddComponent(gameObjectBeingDragged, requiredBindingType);
                }
                default:
                    return null;
            }
        }

        static void InteractiveBindToMissingComponent(PlayableDirector director, TrackAsset bindTo, Object objectToBind, Type requiredComponentType)
        {
            var gameObjectBeingDragged = objectToBind as GameObject;
            Debug.Assert(gameObjectBeingDragged != null, "The object being dragged was detected as being a GameObject");

            string typeNameOfComponent = requiredComponentType.ToString().Split(".".ToCharArray()).Last();
            var bindMenu = new GenericMenu();
            bindMenu.AddItem(
                EditorGUIUtility.TextContent("Create " + typeNameOfComponent + " on " + gameObjectBeingDragged.name),
                false,
                nullParam => Bind(director, bindTo, Undo.AddComponent(gameObjectBeingDragged, requiredComponentType)),
                null);

            bindMenu.AddSeparator("");
            bindMenu.AddItem(EditorGUIUtility.TrTextContent("Cancel"), false, userData => { }, null);
            bindMenu.ShowAsContext();
        }
    }
}
