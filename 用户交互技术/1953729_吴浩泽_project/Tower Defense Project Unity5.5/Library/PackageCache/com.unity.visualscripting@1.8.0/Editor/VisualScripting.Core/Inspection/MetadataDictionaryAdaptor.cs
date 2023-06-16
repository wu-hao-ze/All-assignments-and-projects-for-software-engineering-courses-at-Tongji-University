using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class MetadataDictionaryAdaptor : MetadataCollectionAdaptor
    {
        public MetadataDictionaryAdaptor(Metadata metadata, Inspector parentDrawer) : base(metadata, parentDrawer)
        {
            this.metadata = metadata;

            metadata.valueChanged += (previousValue) =>
            {
                if (!metadata.isDictionary)
                {
                    throw new InvalidOperationException("Metadata for dictionary adaptor is not a dictionary: " + metadata);
                }

                if (metadata.value == null)
                {
                    metadata.value = ConstructDictionary();
                }

                newKeyMetadata?.Unlink();

                newValueMetadata?.Unlink();

                newKeyMetadata = metadata.Object("newKey", ConstructKey(), metadata.dictionaryKeyType);
                newValueMetadata = metadata.Object("newValue", ConstructValue(), metadata.dictionaryValueType);
            };
        }

        public event Action<object, object> itemAdded;

        private Metadata newKeyMetadata;
        private Metadata newValueMetadata;

        protected Metadata metadata { get; private set; }

        protected virtual IDictionary ConstructDictionary()
        {
            try
            {
                return (IDictionary)Activator.CreateInstance(metadata.dictionaryType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not create dictionary instance of type '{metadata.dictionaryType}'.", ex);
            }
        }

        protected virtual object ConstructKey()
        {
            if (metadata.dictionaryKeyType.IsValueType)
            {
                return Activator.CreateInstance(metadata.dictionaryKeyType);
            }
            else
            {
                return null;
            }
        }

        protected virtual object ConstructValue()
        {
            if (metadata.dictionaryValueType.IsValueType)
            {
                return Activator.CreateInstance(metadata.dictionaryValueType);
            }
            else
            {
                return null;
            }
        }

        private const float spaceBetweenKeyAndValue = 5;
        private const float itemPadding = 2;

        #region Manipulation

        public object this[object key]
        {
            get
            {
                return ((IDictionary)metadata)[key];
            }
            set
            {
                metadata.RecordUndo();
                ((IDictionary)metadata)[key] = value;
            }
        }

        public override int Count => metadata.Count + 1;

        public override void Add()
        {
            if (!CanAdd())
            {
                return;
            }

            var newKey = newKeyMetadata.value;
            var newValue = newValueMetadata.value;

            metadata.RecordUndo();
            metadata.Add(newKey, newValue);

            itemAdded?.Invoke(newKey, newValue);

            parentInspector.SetHeightDirty();
        }

        public override void Clear()
        {
            metadata.RecordUndo();
            metadata.Clear();

            parentInspector.SetHeightDirty();
        }

        public override void Insert(int index)
        {
        }

        public override void Remove(int index)
        {
            metadata.RecordUndo();
            metadata.Remove(metadata.KeyMetadata(index).value);
            parentInspector.SetHeightDirty();
        }

        public override void Move(int sourceIndex, int destinationIndex)
        {
        }

        public override void Duplicate(int index)
        {
        }

        protected bool CanAdd()
        {
            var newKey = newKeyMetadata.value;

            if (newKey == null)
            {
                EditorUtility.DisplayDialog("New Dictionary Item", "A dictionary key cannot be null.", "OK");
                return false;
            }

            if (metadata.Contains(newKeyMetadata.value))
            {
                EditorUtility.DisplayDialog("New Dictionary Item", "An item with the same key already exists.", "OK");
                return false;
            }

            return true;
        }

        public override bool CanDrag(int index)
        {
            return metadata.isOrderedDictionary && (index != Count - 1);
        }

        public override bool CanRemove(int index)
        {
            return index != Count - 1;
        }

        #endregion

        #region Drawing

        public override float GetItemAdaptiveWidth(int index)
        {
            // TODO
            return 100;
        }

        public override float GetItemHeight(float width, int index)
        {
            if (index == Count - 1)
            {
                return GetNewItemHeight(width);
            }
            else
            {
                return GetItemHeight(metadata.KeyMetadata(index), metadata.ValueMetadata(index), width);
            }
        }

        public override void DrawItem(Rect position, int index)
        {
            if (index == Count - 1)
            {
                DrawNewItem(position);
            }
            else
            {
                OnItemGUI(metadata.KeyMetadata(index), metadata.ValueMetadata(index), position, false);
            }
        }

        private float GetNewItemHeight(float width)
        {
            var height = 0f;
            height += EditorGUIUtility.singleLineHeight;
            height += GetItemHeight(newKeyMetadata, newValueMetadata, width);
            return height;
        }

        private void DrawNewItem(Rect position)
        {
            var newLabelPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            var newItemPosition = new Rect
                (
                position.x,
                newLabelPosition.yMax,
                position.width,
                GetItemHeight(newKeyMetadata, newValueMetadata, position.width)
                );

            GUI.Label(newLabelPosition, "New Item: ");
            OnItemGUI(newKeyMetadata, newValueMetadata, newItemPosition, true);
        }

        private float GetKeyHeight(Metadata keyMetadata, float keyWidth)
        {
            return LudiqGUI.GetInspectorHeight(parentInspector, keyMetadata, keyWidth, GUIContent.none);
        }

        private float GetValueHeight(Metadata valueMetadata, float valueWidth)
        {
            return LudiqGUI.GetInspectorHeight(parentInspector, valueMetadata, valueWidth, GUIContent.none);
        }

        private float GetKeyWidth(float width)
        {
            return (width - spaceBetweenKeyAndValue) / 2;
        }

        private float GetValueWidth(float width)
        {
            return (width - spaceBetweenKeyAndValue) / 2;
        }

        private float GetItemHeight(Metadata keyMetadata, Metadata valueMetadata, float width)
        {
            return Mathf.Max(GetKeyHeight(keyMetadata, GetKeyWidth(width)), GetValueHeight(valueMetadata, GetValueWidth(width))) + (itemPadding * 2);
        }

        private void OnKeyGUI(Metadata keyMetadata, Rect keyPosition)
        {
            LudiqGUI.Inspector(keyMetadata, keyPosition, GUIContent.none);
        }

        private void OnValueGUI(Metadata valueMetadata, Rect valuePosition)
        {
            LudiqGUI.Inspector(valueMetadata, valuePosition, GUIContent.none);
        }

        private void OnItemGUI(Metadata keyMetadata, Metadata valueMetadata, Rect position, bool editableKey)
        {
            var keyPosition = new Rect
                (
                position.x + itemPadding,
                position.y + itemPadding,
                GetKeyWidth(position.width),
                GetKeyHeight(keyMetadata, GetKeyWidth(position.width))
                );

            var valuePosition = new Rect
                (
                keyPosition.xMax + spaceBetweenKeyAndValue,
                position.y + itemPadding,
                GetValueWidth(position.width),
                GetValueHeight(valueMetadata, GetValueWidth(position.width))
                );

            EditorGUI.BeginDisabledGroup(!editableKey);
            OnKeyGUI(keyMetadata, keyPosition);
            EditorGUI.EndDisabledGroup();

            OnValueGUI(valueMetadata, valuePosition);
        }

        #endregion
    }
}
