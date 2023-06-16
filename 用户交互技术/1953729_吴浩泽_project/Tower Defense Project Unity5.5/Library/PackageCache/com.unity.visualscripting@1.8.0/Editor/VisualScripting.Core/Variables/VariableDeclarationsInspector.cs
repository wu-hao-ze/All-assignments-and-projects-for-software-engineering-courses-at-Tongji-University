using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(VariableDeclarations))]
    public class VariableDeclarationsInspector : Inspector
    {
        public VariableDeclarationsInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            base.Initialize();

            adaptor = new ListAdaptor(metadata["collection"], this);

#pragma warning disable 618
            kind = metadata.GetAttribute<VariableKindAttribute>()?.kind;
#pragma warning restore 618
            kind ??= (metadata.value as VariableDeclarations)?.Kind;
        }

        private ListAdaptor adaptor;
        private string newName;
        private bool highlightPlaceholder;
        private bool highlightNewNameField;

        public VariableKind? kind { get; set; }

        protected override void OnGUI(Rect drawerPosition, GUIContent label)
        {
            adaptor.Field(drawerPosition, GUIContent.none);

            drawerPosition.x = 0;
            drawerPosition.width = LudiqGUIUtility.currentInspectorWidthWithoutScrollbar;

            var newNamePosition = new Rect
                (
                drawerPosition.x,
                drawerPosition.yMax - 20,
                drawerPosition.width - Styles.addButtonWidth,
                18
                );

            if (adaptor.Count == 0)
            {
                newNamePosition.y -= 1;
                newNamePosition.height += 1;
            }

            newNamePosition.height += 1;

            OnNewNameGUI(newNamePosition);
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            return adaptor.GetHeight(width, GUIContent.none);
        }

        private void OnNewNameGUI(Rect newNamePosition)
        {
            EditorGUI.BeginChangeCheck();

            GUI.SetNextControlName(newNameFieldControl);
            newName = EditorGUI.TextField(newNamePosition, newName, highlightNewNameField ? Styles.newNameFieldHighlighted : Styles.newNameField);

            var e = UnityEngine.Event.current;
            if (GUI.GetNameOfFocusedControl() == newNameFieldControl && e.type == EventType.KeyUp && e.keyCode == KeyCode.Return)
            {
                adaptor.Add();
                GUI.FocusControl(newNameFieldControl);
                GUI.changed = true;
            }

            if (EditorGUI.EndChangeCheck())
            {
                highlightNewNameField = false;
                highlightPlaceholder = false;
            }

            if (string.IsNullOrEmpty(newName))
            {
                GUI.Label(newNamePosition, "(New Variable Name)", highlightPlaceholder ? Styles.placeholderHighlighted : Styles.placeholder);
            }
        }

        private const string newNameFieldControl = "newNameField";

        public static class Styles
        {
            static Styles()
            {
                newNameField = new GUIStyle(EditorStyles.textField);
                newNameField.alignment = TextAnchor.MiddleRight;
                newNameField.padding = new RectOffset(4, 4, 0, 0);

                placeholder = new GUIStyle(EditorStyles.label);
                placeholder.normal.textColor = EditorStyles.centeredGreyMiniLabel.normal.textColor;
                placeholder.alignment = newNameField.alignment;
                placeholder.padding = newNameField.padding;

                placeholderHighlighted = new GUIStyle(placeholder);
                placeholderHighlighted.normal.textColor = Color.red;
                placeholderHighlighted.active.textColor = Color.red;
                placeholderHighlighted.focused.textColor = Color.red;
                placeholderHighlighted.hover.textColor = Color.red;

                newNameFieldHighlighted = new GUIStyle(newNameField);
                newNameFieldHighlighted.normal.textColor = Color.red;
                newNameFieldHighlighted.active.textColor = Color.red;
                newNameFieldHighlighted.focused.textColor = Color.red;
                newNameFieldHighlighted.hover.textColor = Color.red;
            }

            public static readonly float invocationSpacing = 7;
            public static readonly float addButtonWidth = 29;
            public static readonly GUIStyle newNameField;
            public static readonly GUIStyle newNameFieldHighlighted;
            public static readonly GUIStyle placeholder;
            public static readonly GUIStyle placeholderHighlighted;
        }

        internal class ListAdaptor : MetadataListAdaptor
        {
            public ListAdaptor(Metadata metadata, VariableDeclarationsInspector parentInspector) : base(metadata, parentInspector)
            {
                this.parentInspector = parentInspector;
                alwaysDragAndDrop = true;
            }

            public new readonly VariableDeclarationsInspector parentInspector;

            protected override bool CanDrop(object item)
            {
                var variableDeclaration = (VariableDeclaration)item;

                if (((VariableDeclarations)parentInspector.metadata.value).IsDefined(variableDeclaration.name))
                {
                    EditorUtility.DisplayDialog("Dragged Variable", "A variable with the same name already exists.", "OK");
                    return false;
                }

                return base.CanDrop(item);
            }

            protected override bool CanAdd()
            {
                if (StringUtility.IsNullOrWhiteSpace(parentInspector.newName))
                {
                    parentInspector.highlightPlaceholder = true;
                    EditorUtility.DisplayDialog("New Variable", "Please enter a variable name.", "OK");
                    return false;
                }
                else if (((VariableDeclarations)parentInspector.metadata.value).IsDefined(parentInspector.newName))
                {
                    parentInspector.highlightNewNameField = true;
                    EditorUtility.DisplayDialog("New Variable", "A variable with the same name already exists.", "OK");
                    return false;
                }

                return true;
            }

            protected override object ConstructItem()
            {
                var newItem = new VariableDeclaration(parentInspector.newName, null);
                parentInspector.newName = null;
                parentInspector.highlightPlaceholder = false;
                parentInspector.highlightNewNameField = false;
                return newItem;
            }
        }
    }
}
