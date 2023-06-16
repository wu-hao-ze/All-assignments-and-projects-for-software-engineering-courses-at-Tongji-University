using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace UnityEditor.Timeline
{
    [CustomEditor(typeof(TimelineAsset)), CanEditMultipleObjects]
    class TimelineAssetInspector : Editor
    {
        const int k_MinWidth = 140;

        internal static class Styles
        {
            public static readonly GUIContent FrameRate = L10n.TextContent("Frame Rate", "The frame rate at which this sequence updates");
            public static readonly GUIContent DurationMode = L10n.TextContent("Duration Mode", "Specified how the duration of the sequence is calculated");
            public static readonly GUIContent Duration = L10n.TextContent("Duration", "The length of the sequence");
            public static readonly GUIContent HeaderTitleMultiselection = L10n.TextContent("Timeline Assets");
            public static readonly GUIContent IgnorePreviewWarning = L10n.TextContent("When ignoring preview, the Timeline window will modify the scene when this timeline is opened.");
            public static readonly GUIContent ScenePreviewLabel = L10n.TextContent("Scene Preview");
        }

        SerializedProperty m_FrameRateProperty;
        SerializedProperty m_DurationModeProperty;
        SerializedProperty m_FixedDurationProperty;
        SerializedProperty m_ScenePreviewProperty;

        void InitializeProperties()
        {
            var editorSettings = serializedObject.FindProperty("m_EditorSettings");
            m_FrameRateProperty = editorSettings.FindPropertyRelative("m_Framerate");
            m_DurationModeProperty = serializedObject.FindProperty("m_DurationMode");
            m_FixedDurationProperty = serializedObject.FindProperty("m_FixedDuration");
            m_ScenePreviewProperty = editorSettings.FindPropertyRelative("m_ScenePreview");
        }

        public void OnEnable()
        {
            InitializeProperties();
        }

        internal override bool IsEnabled()
        {
            return !FileUtil.HasReadOnly(targets) && base.IsEnabled();
        }

        protected override void OnHeaderGUI()
        {
            string headerTitle;
            if (targets.Length == 1)
                headerTitle = target.name;
            else
                headerTitle = targets.Length + " " + Styles.HeaderTitleMultiselection.text;

            DrawHeaderGUI(this, headerTitle, 0);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_FrameRateProperty, Styles.FrameRate);
#if TIMELINE_FRAMEACCURATE
            if (EditorGUI.EndChangeCheck())
            {
                ResetFrameLockedPlayback(targets);
            }
#else
            EditorGUI.EndChangeCheck();
#endif

            var frameRate = m_FrameRateProperty.doubleValue;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(m_DurationModeProperty, Styles.DurationMode, GUILayout.MinWidth(k_MinWidth));

            var durationMode = (TimelineAsset.DurationMode)m_DurationModeProperty.enumValueIndex;
            var inputEvent = InputEvent.None;
            if (durationMode == TimelineAsset.DurationMode.FixedLength)
                TimelineInspectorUtility.TimeField(m_FixedDurationProperty, Styles.Duration, false, frameRate, double.Epsilon, TimelineClip.kMaxTimeValue * 2, ref inputEvent);
            else
            {
                var isMixed = targets.Length > 1;
                TimelineInspectorUtility.TimeField(Styles.Duration, ((TimelineAsset)target).duration, true, isMixed, frameRate, double.MinValue, double.MaxValue, ref inputEvent);
            }

            DrawIgnorePreviewProperty();

            var changed = EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();
            if (changed)
                TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
        }

        void DrawIgnorePreviewProperty()
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(m_ScenePreviewProperty, Styles.ScenePreviewLabel, GUILayout.MinWidth(k_MinWidth));
            }
            var changed = EditorGUI.EndChangeCheck();

            if (changed && TimelineWindow.instance && TimelineWindow.instance.state != null && ContainsMasterAsset(targets))
                ResetWindowState(TimelineWindow.instance.state);

            if (!m_ScenePreviewProperty.boolValue || m_ScenePreviewProperty.hasMultipleDifferentValues)
                EditorGUILayout.HelpBox(Styles.IgnorePreviewWarning.text, MessageType.Warning);
        }

        static void ResetWindowState(WindowState state)
        {
            state.Reset();
            state.Stop();
            state.masterSequence.viewModel.windowTime = 0.0;
            if (state.masterSequence.director != null) state.masterSequence.director.time = 0.0;
        }

        static bool ContainsMasterAsset(Object[] asset)
        {
            return asset != null && asset.Any(tl => tl == TimelineWindow.instance.state.masterSequence.asset);
        }

#if TIMELINE_FRAMEACCURATE
        static void ResetFrameLockedPlayback(Object[] asset)
        {
            if (TimelineWindow.instance != null
                && TimelineWindow.instance.state != null
                && TimelinePreferences.instance.playbackLockedToFrame
                && ContainsMasterAsset(asset))
            {
                TimelineEditor.RefreshPreviewPlay();
            }
        }

#endif
    }
}
