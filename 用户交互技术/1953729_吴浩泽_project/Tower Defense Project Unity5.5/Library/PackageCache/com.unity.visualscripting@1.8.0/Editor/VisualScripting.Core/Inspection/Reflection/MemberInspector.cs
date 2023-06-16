using System;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Member))]
    public sealed class MemberInspector : Inspector
    {
        public MemberInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            metadata.instantiate = true;

            base.Initialize();

            memberFilter = metadata.GetAttribute<MemberFilter>() ?? MemberFilter.Any;
            memberTypeFilter = metadata.GetAttribute<TypeFilter>() ?? TypeFilter.Any;
            direction = ActionDirection.Any;
        }

        private IFuzzyOptionTree GetOptions()
        {
            return new MemberOptionTree(Codebase.GetTypeSetFromAttribute(metadata), memberFilter, memberTypeFilter, direction);
        }

        public static class Styles
        {
            static Styles()
            {
                failurePopup = new GUIStyle(EditorStyles.popup);
                failurePopup.normal.textColor = Color.red;
                failurePopup.active.textColor = Color.red;
                failurePopup.hover.textColor = Color.red;
                failurePopup.focused.textColor = Color.red;
            }

            public static readonly GUIStyle failurePopup;
        }

        #region Metadata

        private Metadata nameMetadata => metadata[nameof(Member.name)];

        private Metadata infoMetadata => metadata[nameof(Member.info)];

        private Metadata targetTypeMetadata => metadata[nameof(Member.targetType)];

        #endregion

        #region Settings

        private ActionDirection direction;

        private ReadOnlyCollection<Type> typeSet;

        private MemberFilter memberFilter;

        private TypeFilter memberTypeFilter;

        private bool expectingBoolean => memberTypeFilter?.ExpectsBoolean ?? false;

        #endregion

        #region Rendering

        protected override float GetHeight(float width, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var popupLabel = new GUIContent("(Nothing)");
            var popupStyle = EditorStyles.popup;

            if (metadata.value != null)
            {
                popupLabel.text = (string)nameMetadata.value;

                if (BoltCore.Configuration.humanNaming && popupLabel.text != null)
                {
                    popupLabel.text = popupLabel.text.Prettify();
                }

                try
                {
                    var member = ((Member)metadata.value);
                    popupLabel.image = member.pseudoDeclaringType.Icon()?[IconSize.Small];
                    popupLabel.text = member.info.DisplayName(direction, expectingBoolean);
                }
                catch
                {
                    popupStyle = Styles.failurePopup;
                }
            }

            if (popupLabel.image != null)
            {
                popupLabel.text = " " + popupLabel.text;
            }

            var newMemberManipulator = (Member)LudiqGUI.FuzzyPopup
                (
                    position,
                    GetOptions,
                    (Member)metadata.value,
                    new GUIContent(popupLabel),
                    popupStyle
                );

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newMemberManipulator;
            }
        }

        #endregion
    }
}
