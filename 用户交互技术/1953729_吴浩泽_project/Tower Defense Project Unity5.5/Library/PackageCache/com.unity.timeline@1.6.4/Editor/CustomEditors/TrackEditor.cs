using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// The user-defined options for drawing a track."
    /// </summary>
    public struct TrackDrawOptions
    {
        /// <summary>
        /// Text that indicates if the track should display an error.
        /// </summary>
        /// <remarks>
        /// If the error text is not empty or null, then the track displays a warning. The error text is used as the tooltip.
        /// </remarks>
        public string errorText { get; set; }

        /// <summary>
        /// The highlight color of the track.
        /// </summary>
        public Color trackColor { get; set; }

        /// <summary>
        /// The minimum height of the track.
        /// </summary>
        public float minimumHeight { get; set; }

        /// <summary>
        /// The icon displayed on the track header.
        /// </summary>
        /// <remarks>
        /// If this value is null, then the default icon for the track is used.
        /// </remarks>
        public Texture2D icon { get; set; }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>Returns <c>true</c> if <paramref name="obj"/> and this instance are the same type and represent the same value.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TrackDrawOptions))
                return false;

            return Equals((TrackDrawOptions)obj);
        }

        /// <summary>
        /// Compares this object with another <c>TrackDrawOptions</c>.
        /// </summary>
        /// <param name="other">The object to compare with.</param>
        /// <returns>Returns true if <c>this</c> and <paramref name="other"/> are equal.</returns>
        public bool Equals(TrackDrawOptions other)
        {
            return errorText == other.errorText &&
                trackColor == other.trackColor &&
                minimumHeight == other.minimumHeight &&
                icon == other.icon;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return HashUtility.CombineHash(
                errorText != null ? errorText.GetHashCode() : 0,
                trackColor.GetHashCode(),
                minimumHeight.GetHashCode(),
                icon != null ? icon.GetHashCode() : 0
            );
        }

        /// <summary>
        /// Compares two <c>TrackDrawOptions</c> objects.
        /// </summary>
        /// <param name="options1">The first object.</param>
        /// <param name="options2">The second object.</param>
        /// <returns>Returns true if they are equal.</returns>
        public static bool operator ==(TrackDrawOptions options1, TrackDrawOptions options2)
        {
            return options1.Equals(options2);
        }

        /// <summary>
        /// Compares two <c>TrackDrawOptions</c> objects.
        /// </summary>
        /// <param name="options1">The first object.</param>
        /// <param name="options2">The second object.</param>
        /// <returns>Returns true if they are not equal.</returns>
        public static bool operator !=(TrackDrawOptions options1, TrackDrawOptions options2)
        {
            return !options1.Equals(options2);
        }
    }


    /// <summary>
    /// The errors displayed for the track binding.
    /// </summary>
    public enum TrackBindingErrors
    {
        /// <summary>
        /// Select no errors.
        /// </summary>
        None = 0,

        /// <summary>
        /// The bound GameObject is disabled.
        /// </summary>
        BoundGameObjectDisabled = 1 << 0,

        /// <summary>
        /// The bound GameObject does not have a valid component.
        /// </summary>
        NoValidComponent = 1 << 1,

        /// <summary>
        /// The bound Object is a disabled Behaviour.
        /// </summary>
        BehaviourIsDisabled = 1 << 2,

        /// <summary>
        /// The bound Object is not of the correct type.
        /// </summary>
        InvalidBinding = 1 << 3,

        /// <summary>
        /// The bound Object is part of a prefab, and not an instance.
        /// </summary>
        PrefabBound = 1 << 4,

        /// <summary>
        /// Select all errors.
        /// </summary>
        All = Int32.MaxValue
    }

    /// <summary>
    /// Use this class to customize track types in the TimelineEditor.
    /// </summary>
    public class TrackEditor
    {
        static readonly string k_BoundGameObjectDisabled = L10n.Tr("The bound GameObject is disabled.");
        static readonly string k_NoValidComponent = L10n.Tr("Could not find appropriate component on this gameObject");
        static readonly string k_RequiredComponentIsDisabled = L10n.Tr("The component is disabled");
        static readonly string k_InvalidBinding = L10n.Tr("The bound object is not the correct type.");
        static readonly string k_PrefabBound = L10n.Tr("The bound object is a Prefab");

        readonly Dictionary<TrackAsset, System.Type> m_BindingCache = new Dictionary<TrackAsset, System.Type>();

        /// <summary>
        /// The default height of a track.
        /// </summary>
        public static readonly float DefaultTrackHeight = 30.0f;

        /// <summary>
        /// The minimum unscaled height of a track.
        /// </summary>
        public static readonly float MinimumTrackHeight = 10.0f;

        /// <summary>
        /// The maximum height of a track.
        /// </summary>
        public static readonly float MaximumTrackHeight = 256.0f;

        /// <summary>
        /// Implement this method to override the default options for drawing a track.
        /// </summary>
        /// <param name="track">The track from which track options are retrieved.</param>
        /// <param name="binding">The binding for the track.</param>
        /// <returns>The options for drawing the track.</returns>
        public virtual TrackDrawOptions GetTrackOptions(TrackAsset track, UnityEngine.Object binding)
        {
            return new TrackDrawOptions()
            {
                errorText = GetErrorText(track, binding, TrackBindingErrors.All),
                minimumHeight = DefaultTrackHeight,
                trackColor = GetTrackColor(track),
                icon = null
            };
        }

        /// <summary>
        /// Gets the error text for the specified track.
        /// </summary>
        /// <param name="track">The track to retrieve options for.</param>
        /// <param name="boundObject">The binding for the track.</param>
        /// <param name="detectErrors">The errors to check for.</param>
        /// <returns>An error to be displayed on the track, or string.Empty if there is no error.</returns>
        public string GetErrorText(TrackAsset track, UnityEngine.Object boundObject, TrackBindingErrors detectErrors)
        {
            if (track == null || boundObject == null)
                return string.Empty;

            var bindingType = GetBindingType(track);
            if (bindingType != null)
            {
                // bound to a prefab asset
                if (HasFlag(detectErrors, TrackBindingErrors.PrefabBound) && PrefabUtility.IsPartOfPrefabAsset(boundObject))
                {
                    return k_PrefabBound;
                }

                // If we are a component, allow for bound game objects (legacy)
                if (typeof(Component).IsAssignableFrom(bindingType))
                {
                    var gameObject = boundObject as GameObject;
                    var component = boundObject as Component;
                    if (component != null)
                        gameObject = component.gameObject;

                    // game object is bound with no component
                    if (HasFlag(detectErrors, TrackBindingErrors.NoValidComponent) && gameObject != null && component == null)
                    {
                        component = gameObject.GetComponent(bindingType);
                        if (component == null)
                        {
                            return k_NoValidComponent;
                        }
                    }

                    // attached gameObject is disables (ignores Activation Track)
                    if (HasFlag(detectErrors, TrackBindingErrors.BoundGameObjectDisabled) && gameObject != null && !gameObject.activeInHierarchy)
                    {
                        return k_BoundGameObjectDisabled;
                    }

                    // component is disabled
                    var behaviour = component as Behaviour;
                    if (HasFlag(detectErrors, TrackBindingErrors.BehaviourIsDisabled) && behaviour != null && !behaviour.enabled)
                    {
                        return k_RequiredComponentIsDisabled;
                    }

                    // mismatched binding
                    if (HasFlag(detectErrors, TrackBindingErrors.InvalidBinding) && component != null && !bindingType.IsAssignableFrom(component.GetType()))
                    {
                        return k_InvalidBinding;
                    }
                }
                // Mismatched binding (non-component)
                else if (HasFlag(detectErrors, TrackBindingErrors.InvalidBinding) && !bindingType.IsAssignableFrom(boundObject.GetType()))
                {
                    return k_InvalidBinding;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the color information of a track.
        /// </summary>
        /// <param name="track"></param>
        /// <returns>Returns the color for the specified track.</returns>
        public Color GetTrackColor(TrackAsset track)
        {
            return TrackResourceCache.GetTrackColor(track);
        }

        /// <summary>
        /// Gets the binding type for a track.
        /// </summary>
        /// <param name="track">The track to retrieve the binding type from.</param>
        /// <returns>Returns the binding type for the specified track. Returns null if the track does not have binding.</returns>
        public System.Type GetBindingType(TrackAsset track)
        {
            if (track == null)
                return null;

            if (m_BindingCache.TryGetValue(track, out var result))
                return result;

            result = track.outputs.Select(x => x.outputTargetType).FirstOrDefault();
            m_BindingCache[track] = result;
            return result;
        }

        /// <summary>
        /// Callback for when a track is created.
        /// </summary>
        /// <param name="track">The track that is created.</param>
        /// <param name="copiedFrom">The source that the track is copied from. This can be set to null if the track is not a copy.</param>
        public virtual void OnCreate(TrackAsset track, TrackAsset copiedFrom)
        {
        }

        /// <summary>
        /// Callback for when a track is changed.
        /// </summary>
        /// <param name="track">The track that is changed.</param>
        public virtual void OnTrackChanged(TrackAsset track)
        {
        }

        static bool HasFlag(TrackBindingErrors errors, TrackBindingErrors flag)
        {
            return (errors & flag) != 0;
        }

        /// <summary>
        /// Override this method to validate if a binding for <paramref name="track"/>
        /// can be determined from <paramref name="candidate"/>.
        ///
        /// The default implementation of this method will return true if
        /// - <paramref name="candidate"/> is not null or,
        /// - <paramref name="candidate"/> is not part of a Prefab Asset or,
        /// - <paramref name="candidate"/> is a Component that can be bound to <paramref name="track"/>
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="track">TBD</param>
        /// <returns>True if a binding can be determined from <paramref name="candidate"/>.</returns>
        /// <seealso cref="UnityEngine.Timeline.TrackBindingTypeAttribute"/>
        /// <seealso cref="UnityEngine.Timeline.TrackAsset"/>
        public virtual bool IsBindingAssignableFrom(UnityEngine.Object candidate, TrackAsset track)
        {
            var action = BindingUtility.GetBindingAction(GetBindingType(track), candidate);
            return action != BindingUtility.BindingAction.DoNotBind;
        }

        /// <summary>
        /// Override this method to determine which object to bind to <paramref name="track"/>.
        /// A binding object should be determined from <paramref name="candidate"/>.
        ///
        /// By default, the `TrackBindingType` attribute from <paramref name="track"/> will be used to determine the binding.
        /// </summary>
        /// <param name="candidate">The source object from which a track binding should be determined.</param>
        /// <param name="track">The track to bind an object to.</param>
        /// <returns>The object to bind to <paramref name="track"/>.</returns>
        /// <seealso cref="UnityEngine.Timeline.TrackBindingTypeAttribute"/>
        /// <seealso cref="UnityEngine.Timeline.TrackAsset"/>
        public virtual UnityEngine.Object GetBindingFrom(UnityEngine.Object candidate, TrackAsset track)
        {
            Type bindingType = GetBindingType(track);
            BindingUtility.BindingAction action = BindingUtility.GetBindingAction(bindingType, candidate);
            return BindingUtility.GetBinding(action, candidate, bindingType);
        }
    }

    static class TrackEditorExtension
    {
        public static bool SupportsBindingAssign(this TrackEditor editor)
        {
            return TypeUtility.HasOverrideMethod(editor.GetType(), nameof(TrackEditor.GetBindingFrom));
        }

        public static void OnCreate_Safe(this TrackEditor editor, TrackAsset track, TrackAsset copiedFrom)
        {
            try
            {
                editor.OnCreate(track, copiedFrom);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static TrackDrawOptions GetTrackOptions_Safe(this TrackEditor editor, TrackAsset track, UnityEngine.Object binding)
        {
            try
            {
                return editor.GetTrackOptions(track, binding);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return CustomTimelineEditorCache.GetDefaultTrackEditor().GetTrackOptions(track, binding);
            }
        }

        public static UnityEngine.Object GetBindingFrom_Safe(this TrackEditor editor, UnityEngine.Object candidate, TrackAsset track)
        {
            try
            {
                return editor.GetBindingFrom(candidate, track);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return candidate;
            }
        }

        public static bool IsBindingAssignableFrom_Safe(this TrackEditor editor, UnityEngine.Object candidate, TrackAsset track)
        {
            try
            {
                return editor.IsBindingAssignableFrom(candidate, track);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public static void OnTrackChanged_Safe(this TrackEditor editor, TrackAsset track)
        {
            try
            {
                editor.OnTrackChanged(track);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
