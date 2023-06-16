using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
#if !UNITY_2020_2_OR_NEWER
using L10n = UnityEditor.Timeline.L10n;
#endif

/// <summary>
/// Store the editor preferences for Timeline.
/// </summary>
[FilePath("TimelinePreferences.asset", FilePathAttribute.Location.PreferencesFolder)]
public class TimelinePreferences : ScriptableSingleton<TimelinePreferences>
{
    /// <summary>
    /// The time unit used by the Timeline Editor when displaying time values.
    /// </summary>
    [SerializeField]
    public TimeFormat timeFormat;

    /// <summary>
    /// Define the time unit for the timeline window.
    /// true : frame unit.
    /// false : timecode unit.
    /// </summary>
    [NonSerialized, Obsolete("timeUnitInFrame is deprecated. Use timeFormat instead", false)]
    public bool timeUnitInFrame;

    /// <summary>
    /// Draw the waveforms for all audio clips.
    /// </summary>
    [SerializeField]
    public bool showAudioWaveform = true;

    /// <summary>
    /// Allow the users to hear audio while scrubbing on audio clip.
    /// </summary>
    [SerializeField]
    bool m_AudioScrubbing;

    /// <summary>
    /// Enables audio scrubbing when moving the playhead.
    /// </summary>
    public bool audioScrubbing
    {
        get { return m_AudioScrubbing; }
        set
        {
            if (m_AudioScrubbing != value)
            {
                m_AudioScrubbing = value;
                TimelinePlayable.muteAudioScrubbing = !value;
                TimelineEditor.Refresh(RefreshReason.ContentsModified);
            }
        }
    }

    /// <summary>
    /// Enable Snap to Frame to manipulate clips and align them on frames.
    /// </summary>
    [SerializeField]
    public bool snapToFrame = true;

#if TIMELINE_FRAMEACCURATE
    [SerializeField] bool m_PlaybackLockedToFrame;
#endif
    /// <summary>
    /// Enable Timelines to be evaluated on frame during editor preview.
    /// </summary>
    public bool playbackLockedToFrame
    {
        get
        {
#if TIMELINE_FRAMEACCURATE
            return m_PlaybackLockedToFrame;
#else
            Debug.LogWarning($"PlaybackLockedToFrame is not available for this Unity version");
            return false;
#endif
        }
        set
        {
#if TIMELINE_FRAMEACCURATE
            m_PlaybackLockedToFrame = value;
            TimelineEditor.RefreshPreviewPlay();
#else
            Debug.LogWarning($"PlaybackLockedToFrame is not available for this Unity version");
#endif
        }
    }


    /// <summary>
    /// Enable the ability to snap clips on the edge of another clip.
    /// </summary>
    [SerializeField]
    public bool edgeSnap = true;
    /// <summary>
    /// Behavior of the timeline window during playback.
    /// </summary>
    [SerializeField]
    public PlaybackScrollMode playbackScrollMode = PlaybackScrollMode.None;

    void OnDisable()
    {
        Save();
    }

    /// <summary>
    /// Save the timeline preferences settings file.
    /// </summary>
    public void Save()
    {
        Save(true);
    }

    internal SerializedObject GetSerializedObject()
    {
        return new SerializedObject(this);
    }
}

class TimelinePreferencesProvider : SettingsProvider
{
    SerializedObject m_SerializedObject;
    SerializedProperty m_ShowAudioWaveform;
    SerializedProperty m_TimeFormat;
    SerializedProperty m_SnapToFrame;
    SerializedProperty m_EdgeSnap;
    SerializedProperty m_PlaybackScrollMode;
    SerializedProperty m_PlaybackLockedToFrame;

    internal class Styles
    {
        public static readonly GUIContent TimeUnitLabel = L10n.TextContent("Time Unit", "Define the time unit for the timeline window (Frames, Timecode or Seconds).");
        public static readonly GUIContent ShowAudioWaveformLabel = L10n.TextContent("Show Audio Waveforms", "Draw the waveforms for all audio clips.");
        public static readonly GUIContent AudioScrubbingLabel = L10n.TextContent("Allow Audio Scrubbing", "Allow the users to hear audio while scrubbing on audio clip.");
        public static readonly GUIContent SnapToFrameLabel = L10n.TextContent("Snap To Frame", "Enable Snap to Frame to manipulate clips and align them on frames.");
        public static readonly GUIContent EdgeSnapLabel = L10n.TextContent("Edge Snap", "Enable the ability to snap clips on the edge of another clip.");
        public static readonly GUIContent PlaybackScrollModeLabel = L10n.TextContent("Playback Scrolling Mode", "Define scrolling behavior during playback.");
        public static readonly GUIContent EditorSettingLabel = L10n.TextContent("Timeline Editor Settings", "");
#if TIMELINE_FRAMEACCURATE
        public static readonly GUIContent PlaybackLockedToFrame = L10n.TextContent("Playback Locked To Frame", "Enable Frame Accurate Preview.");
#endif
    }

    public TimelinePreferencesProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
        : base(path, scopes, keywords)
    {
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        TimelinePreferences.instance.Save();
        m_SerializedObject = TimelinePreferences.instance.GetSerializedObject();
        m_ShowAudioWaveform = m_SerializedObject.FindProperty("showAudioWaveform");
        m_TimeFormat = m_SerializedObject.FindProperty("timeFormat");
        m_SnapToFrame = m_SerializedObject.FindProperty("snapToFrame");
        m_EdgeSnap = m_SerializedObject.FindProperty("edgeSnap");
        m_PlaybackScrollMode = m_SerializedObject.FindProperty("playbackScrollMode");
#if TIMELINE_FRAMEACCURATE
        m_PlaybackLockedToFrame = m_SerializedObject.FindProperty("m_PlaybackLockedToFrame");
#endif
    }

    public override void OnGUI(string searchContext)
    {
        m_SerializedObject.Update();
        EditorGUI.BeginChangeCheck();
        using (new SettingsWindow.GUIScope())
        {
            EditorGUILayout.LabelField(Styles.EditorSettingLabel, EditorStyles.boldLabel);
            m_TimeFormat.enumValueIndex = EditorGUILayout.Popup(Styles.TimeUnitLabel, m_TimeFormat.enumValueIndex, m_TimeFormat.enumDisplayNames);
            m_PlaybackScrollMode.enumValueIndex = EditorGUILayout.Popup(Styles.PlaybackScrollModeLabel, m_PlaybackScrollMode.enumValueIndex, m_PlaybackScrollMode.enumNames);
            m_ShowAudioWaveform.boolValue = EditorGUILayout.Toggle(Styles.ShowAudioWaveformLabel, m_ShowAudioWaveform.boolValue);
            TimelinePreferences.instance.audioScrubbing = EditorGUILayout.Toggle(Styles.AudioScrubbingLabel, TimelinePreferences.instance.audioScrubbing);
            m_SnapToFrame.boolValue = EditorGUILayout.Toggle(Styles.SnapToFrameLabel, m_SnapToFrame.boolValue);
            m_EdgeSnap.boolValue = EditorGUILayout.Toggle(Styles.EdgeSnapLabel, m_EdgeSnap.boolValue);
#if TIMELINE_FRAMEACCURATE
            m_PlaybackLockedToFrame.boolValue = EditorGUILayout.Toggle(Styles.PlaybackLockedToFrame, m_PlaybackLockedToFrame.boolValue);
#endif
        }
        if (EditorGUI.EndChangeCheck())
        {
            m_SerializedObject.ApplyModifiedProperties();
            TimelinePreferences.instance.Save();
            TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
            TimelineEditor.RefreshPreviewPlay();
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateTimelineProjectSettingProvider()
    {
        var provider = new TimelinePreferencesProvider("Preferences/Timeline", SettingsScope.User, GetSearchKeywordsFromGUIContentProperties<Styles>());
        return provider;
    }
}
