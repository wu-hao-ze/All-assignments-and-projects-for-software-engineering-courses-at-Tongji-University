using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
#if !UNITY_2020_2_OR_NEWER
using L10n = UnityEditor.Timeline.L10n;
#endif

/// <summary>
/// Store the settings for Timeline that will be stored with the Unity Project.
/// </summary>
[FilePath("ProjectSettings/TimelineSettings.asset", FilePathAttribute.Location.ProjectFolder)]
public class TimelineProjectSettings : ScriptableSingleton<TimelineProjectSettings>
{
    /// <summary>
    /// Define the default framerate when a Timeline asset is created.
    /// </summary>
    [HideInInspector, Obsolete("assetDefaultFramerate has been deprecated. Use defaultFrameRate instead.")]
    public float assetDefaultFramerate = (float)TimelineAsset.EditorSettings.kDefaultFrameRate;

    [SerializeField, FrameRateField, FormerlySerializedAs("assetDefaultFramerate")]
    private double m_DefaultFrameRate = TimelineAsset.EditorSettings.kDefaultFrameRate;
    /// <summary>
    /// Defines the default frame rate when a Timeline asset is created from the project window.
    /// </summary>

    public double defaultFrameRate
    {
#pragma warning disable 0618
        get
        {
            if (m_DefaultFrameRate != assetDefaultFramerate)
            {
                return assetDefaultFramerate;
            }
            return m_DefaultFrameRate;
        }
        set
        {
            m_DefaultFrameRate = value;
            assetDefaultFramerate = (float)value;
        }
#pragma warning restore 0618
    }

    void OnDisable()
    {
        Save();
    }

    /// <summary>
    /// Save the timeline project settings file in the project directory.
    /// </summary>
    public void Save()
    {
        Save(true);
    }

    internal SerializedObject GetSerializedObject()
    {
        return new SerializedObject(this);
    }

    private void OnValidate()
    {
#pragma warning disable 0618
        assetDefaultFramerate = (float)m_DefaultFrameRate;
#pragma warning restore 0618
    }
}

class TimelineProjectSettingsProvider : SettingsProvider
{
    SerializedObject m_SerializedObject;
    SerializedProperty m_Framerate;

    private class Styles
    {
        public static readonly GUIContent DefaultFramerateLabel = L10n.TextContent("Default frame rate", "The default frame rate for new Timeline assets.");
        public static readonly GUIContent TimelineAssetLabel = L10n.TextContent("Timeline Asset", "");
        public static readonly string WarningString = L10n.Tr("Locking playback cannot be enabled for this frame rate.");
    }

    public TimelineProjectSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
        : base(path, scopes, keywords) { }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        TimelineProjectSettings.instance.Save();
        m_SerializedObject = TimelineProjectSettings.instance.GetSerializedObject();
        m_Framerate = m_SerializedObject.FindProperty("m_DefaultFrameRate");
    }

    public override void OnGUI(string searchContext)
    {
        using (new SettingsWindow.GUIScope())
        {
            m_SerializedObject.Update();

            EditorGUILayout.LabelField(Styles.TimelineAssetLabel, EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            m_Framerate.doubleValue = FrameRateDrawer.FrameRateField(m_Framerate.doubleValue, Styles.DefaultFramerateLabel,
                EditorGUILayout.GetControlRect(), out bool frameRateIsValid);
            if (EditorGUI.EndChangeCheck())
            {
                m_SerializedObject.ApplyModifiedProperties();
                TimelineProjectSettings.instance.Save();
            }
#if TIMELINE_FRAMEACCURATE
            if (!frameRateIsValid && TimelinePreferences.instance.playbackLockedToFrame)
                EditorGUILayout.HelpBox(Styles.WarningString, MessageType.Warning);
#endif
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateTimelineProjectSettingProvider()
    {
        var provider = new TimelineProjectSettingsProvider("Project/Timeline", SettingsScope.Project, GetSearchKeywordsFromGUIContentProperties<Styles>());
        return provider;
    }
}
