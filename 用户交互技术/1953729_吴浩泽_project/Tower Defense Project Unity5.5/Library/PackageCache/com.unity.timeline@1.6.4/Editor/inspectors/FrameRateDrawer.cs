using System;
using UnityEngine;
using UnityEngine.Timeline;
using TimelineEditorSettings = UnityEngine.Timeline.TimelineAsset.EditorSettings;
#if TIMELINE_FRAMEACCURATE
using UnityEngine.Playables;
#endif

namespace UnityEditor.Timeline
{
    [CustomPropertyDrawer(typeof(FrameRateFieldAttribute), true)]
    class FrameRateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var frameRateAttribute = attribute as FrameRateFieldAttribute;
            if (frameRateAttribute == null)
                return;
            EditorGUI.BeginProperty(position, label, property);
            property.doubleValue = FrameRateField(property.doubleValue, label, position, out bool frameRateIsValid);
            EditorGUI.EndProperty();
#if TIMELINE_FRAMEACCURATE
            if (!frameRateIsValid && TimelinePreferences.instance.playbackLockedToFrame)
                EditorGUILayout.HelpBox(
                    L10n.Tr("Locking playback cannot be enabled for this frame rate."),
                    MessageType.Warning);
#endif
        }

        public static double FrameRateField(double frameRate, GUIContent label, Rect position, out bool isValid)
        {
            double frameRateDouble = FrameRateDisplayUtility.RoundFrameRate(frameRate);
            FrameRate frameRateObj = TimeUtility.GetClosestFrameRate(frameRateDouble);
            isValid = frameRateObj.IsValid();
            TimeUtility.ToStandardFrameRate(frameRateObj, out StandardFrameRates option);

            position = EditorGUI.PrefixLabel(position, label);
            Rect posPopup = new Rect(position.x, position.y, position.width / 2, position.height);
            Rect posFloatField = new Rect(posPopup.xMax, position.y, position.width / 2, position.height);
            using (var checkOption = new EditorGUI.ChangeCheckScope())
            {
                option = (StandardFrameRates)EditorGUI.Popup(posPopup, (int)option,
                    FrameRateDisplayUtility.GetDefaultFrameRatesLabels(option));

                if (checkOption.changed)
                {
                    isValid = true;
                    return TimeUtility.ToFrameRate(option).rate;
                }
            }

            using (var checkFrame = new EditorGUI.ChangeCheckScope())
            {
                frameRateDouble = Math.Abs(EditorGUI.DoubleField(posFloatField, frameRateDouble));
                frameRateObj = TimeUtility.GetClosestFrameRate(frameRateDouble);
                if (checkFrame.changed)
                {
                    isValid = frameRateObj.IsValid();
                    return isValid ? frameRateObj.rate : frameRateDouble;
                }
            }

            return frameRateDouble;
        }
    }
}
