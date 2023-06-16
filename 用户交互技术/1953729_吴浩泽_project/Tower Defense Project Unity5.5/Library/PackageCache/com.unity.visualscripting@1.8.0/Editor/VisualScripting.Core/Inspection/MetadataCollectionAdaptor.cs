using System;
using Unity.VisualScripting.ReorderableList;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class MetadataCollectionAdaptor : IReorderableListAdaptor
    {
        protected MetadataCollectionAdaptor(Metadata metadata, Inspector parentInspector)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (parentInspector == null)
            {
                throw new ArgumentNullException(nameof(parentInspector));
            }

            this.metadata = metadata;
            this.parentInspector = parentInspector;

            var wideAttribute = metadata.GetAttribute<InspectorWideAttribute>();

            if (wideAttribute != null)
            {
                widthMode = wideAttribute.toEdge ? WidthMode.Edge : WidthMode.Wide;
            }
            else
            {
                widthMode = WidthMode.Thin;
            }

            listControl = new ReorderableListControl();
            listControl.ContainerStyle = new GUIStyle(ReorderableListStyles.Container);
            listControl.FooterButtonStyle = new GUIStyle(ReorderableListStyles.FooterButton);
            listControl.ItemButtonStyle = new GUIStyle(ReorderableListStyles.ItemButton);

            listControl.ItemRemoving += ListControlOnItemRemoving;
        }

        private void ListControlOnItemRemoving(object sender, ItemRemovingEventArgs args)
        {
            var warnAttribute = metadata.GetAttribute<WarnBeforeRemovingAttribute>();

            if (warnAttribute != null)
            {
                args.Cancel = !EditorUtility.DisplayDialog(warnAttribute.warningTitle, warnAttribute.warningMessage, "Remove", "Cancel");
            }
        }

        private float itemWidth;

        private WidthMode widthMode;

        private Metadata metadata;

        private ReorderableListControl listControl;

        protected Inspector parentInspector { get; private set; }

        private enum WidthMode
        {
            Thin,

            Wide,

            Edge
        }


        #region Drawing

        public virtual void BeginGUI() { }

        public virtual void EndGUI() { }

        public virtual void DrawItemBackground(Rect position, int index) { }

        protected virtual float GetTitleHeight(float width, GUIContent title)
        {
            if (title == GUIContent.none)
            {
                return 0;
            }

            return 20;
        }

        protected virtual void OnTitleGUI(Rect position, GUIContent title)
        {
            if (title == GUIContent.none)
            {
                return;
            }

            ReorderableListGUI.Title(position, title);
        }

        public float GetItemHeight(int index)
        {
            return GetItemHeight(itemWidth, index);
        }

        public abstract float GetItemHeight(float width, int index);

        public abstract void DrawItem(Rect position, int index);

        public abstract float GetItemAdaptiveWidth(int index);

        public float GetAdaptiveWidth()
        {
            var width = 0f;

            for (int i = 0; i < Count; i++)
            {
                width = Mathf.Max(width, GetItemAdaptiveWidth(i));
            }

            width += 56;

            return width;
        }

        public float GetHeight(float width, GUIContent label)
        {
            if (widthMode == WidthMode.Thin)
            {
                width = Inspector.WidthWithoutLabel(metadata, width, label);
            }
            else if (widthMode == WidthMode.Edge)
            {
                width = LudiqGUIUtility.currentInspectorWidthWithoutScrollbar;
            }

            itemWidth = width - 52; // Approximation of handle + remove button

            var height = 0f;

            if (widthMode != WidthMode.Thin && label != GUIContent.none)
            {
                height += GetTitleHeight(width, label);
            }

            height += listControl.CalculateListHeight(this);
            height -= 10; // Remove default bottom padding

            if (widthMode == WidthMode.Thin)
            {
                height = Inspector.HeightWithLabel(metadata, width, height, label);
            }

            return height;
        }

        public bool Field(Rect position, GUIContent label)
        {
            var y = position.y;

            if (widthMode == WidthMode.Thin)
            {
                position = Inspector.BeginLabeledBlock(metadata, position, label);
            }
            else
            {
                if (widthMode == WidthMode.Edge)
                {
                    position.x = 0;
                    position.width = LudiqGUIUtility.currentInspectorWidthWithoutScrollbar;
                }

                Inspector.BeginLabeledBlock(metadata, position, GUIContent.none);
            }

            position.height += 10; // Restore default padding
            itemWidth = position.width - 52;

            var height = position.height;

            if (widthMode != WidthMode.Thin && label != GUIContent.none)
            {
                var titlePosition = new Rect
                    (
                    position.x,
                    y,
                    position.width,
                    GetTitleHeight(position.width, label)
                    );

                OnTitleGUI(titlePosition, label);

                y += titlePosition.height - 1;
                height -= titlePosition.height;
            }
            else
            {
                height = listControl.CalculateListHeight(this);
            }

            var listPosition = new Rect
                (
                position.x,
                y,
                position.width,
                height
                );

            listControl.Draw(listPosition, this);

            return Inspector.EndBlock(metadata);
        }

        #endregion


        #region Manipulation

        public abstract int Count { get; }

        public abstract bool CanDrag(int index);

        public abstract bool CanRemove(int index);

        public abstract void Add();

        public abstract void Insert(int index);

        public abstract void Duplicate(int index);

        public abstract void Remove(int index);

        public abstract void Move(int sourceIndex, int destIndex);

        public abstract void Clear();

        #endregion
    }
}
