using System;
using System.Collections;
using Unity.VisualScripting.ReorderableList;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public class MetadataListAdaptor : MetadataCollectionAdaptor, IReorderableListDropTarget
    {
        public MetadataListAdaptor(Metadata metadata, Inspector parentInspector) : base((Metadata)metadata, parentInspector)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            this.metadata = metadata;

            metadata.valueChanged += (previousValue) =>
            {
                if (!metadata.isList)
                {
                    throw new InvalidOperationException("Metadata for list adaptor is not a list: " + metadata);
                }

                if (metadata.value == null)
                {
                    metadata.value = ConstructList();
                }
            };
        }

        public event Action<object> itemAdded;

        public Metadata metadata { get; private set; }

        protected virtual IList ConstructList()
        {
            if (metadata.listType.IsArray)
            {
                return Array.CreateInstance(metadata.listElementType, 0);
            }
            else
            {
                try
                {
                    return (IList)metadata.listType.Instantiate(false);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Could not create list instance of type '{metadata.listType}'.", ex);
                }
            }
        }

        protected virtual object ConstructItem()
        {
            return metadata.listElementType.TryInstantiate(false);
        }

        #region Manipulation

        public object this[int index]
        {
            get
            {
                return ((IList)metadata)[index];
            }
            set
            {
                metadata.RecordUndo();
                ((IList)metadata)[index] = value;
            }
        }

        public override int Count => metadata.Count;

        public override void Add()
        {
            if (!CanAdd())
            {
                return;
            }

            var newItem = ConstructItem();

            metadata.RecordUndo();
            metadata.Add(newItem);

            itemAdded?.Invoke(newItem);

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
            if (!CanAdd())
            {
                return;
            }

            var newItem = ConstructItem();

            metadata.RecordUndo();
            metadata.Insert(index, newItem);

            itemAdded?.Invoke(newItem);

            parentInspector.SetHeightDirty();
        }

        public override void Remove(int index)
        {
            metadata.RecordUndo();
            metadata.RemoveAt(index);

            parentInspector.SetHeightDirty();
        }

        public override void Move(int sourceIndex, int destinationIndex)
        {
            metadata.RecordUndo();
            metadata.Move(sourceIndex, destinationIndex);
        }

        public override void Duplicate(int index)
        {
            metadata.RecordUndo();
            metadata.Duplicate(index);

            itemAdded?.Invoke(this[index + 1]);

            parentInspector.SetHeightDirty();
        }

        protected virtual bool CanAdd()
        {
            return true;
        }

        public override bool CanDrag(int index)
        {
            return true;
        }

        public override bool CanRemove(int index)
        {
            return true;
        }

        #endregion

        #region Drag & Drop

        private static MetadataListAdaptor selectedList;
        private static object selectedItem;

        public bool CanDropInsert(int insertionIndex)
        {
            if (!ReorderableListControl.CurrentListPosition.Contains(Event.current.mousePosition))
            {
                return false;
            }

            var data = DragAndDrop.GetGenericData(DraggedListItem.TypeName);

            return data is DraggedListItem && metadata.listElementType.IsInstanceOfType(((DraggedListItem)data).item);
        }

        protected virtual bool CanDrop(object item)
        {
            return true;
        }

        public void ProcessDropInsertion(int insertionIndex)
        {
            if (Event.current.type == EventType.DragPerform)
            {
                var draggedItem = (DraggedListItem)DragAndDrop.GetGenericData(DraggedListItem.TypeName);

                if (draggedItem.sourceListAdaptor == this)
                {
                    Move(draggedItem.index, insertionIndex);
                }
                else
                {
                    if (CanDrop(draggedItem.item))
                    {
                        metadata.Insert(insertionIndex, draggedItem.item);

                        itemAdded?.Invoke(draggedItem.item);

                        draggedItem.sourceListAdaptor.Remove(draggedItem.index);
                        selectedList = this;

                        draggedItem.sourceListAdaptor.parentInspector.SetHeightDirty();
                        parentInspector.SetHeightDirty();
                    }
                }

                GUI.changed = true;
                Event.current.Use();
            }
        }

        #endregion

        #region Drawing

        public override float GetItemAdaptiveWidth(int index)
        {
            return LudiqGUI.GetInspectorAdaptiveWidth(metadata[index]);
        }

        public override float GetItemHeight(float width, int index)
        {
            return LudiqGUI.GetInspectorHeight(parentInspector, metadata[index], width, GUIContent.none);
        }

        public bool alwaysDragAndDrop { get; set; } = false;

        public override void DrawItem(Rect position, int index)
        {
            LudiqGUI.Inspector(metadata[index], position, GUIContent.none);

            var item = this[index];

            var controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    // Exclude delete button from draggable position
                    var draggablePosition = ReorderableListGUI.CurrentItemTotalPosition;
                    draggablePosition.xMax = position.xMax + 2;

                    if (Event.current.button == (int)MouseButton.Left && draggablePosition.Contains(Event.current.mousePosition))
                    {
                        selectedList = this;
                        selectedItem = item;

                        if (alwaysDragAndDrop || Event.current.alt)
                        {
                            GUIUtility.hotControl = controlID;
                            Event.current.Use();
                        }
                    }

                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.objectReferences = new UnityObject[0];
                        DragAndDrop.paths = new string[0];
                        DragAndDrop.SetGenericData(DraggedListItem.TypeName, new DraggedListItem(this, index, item));
                        DragAndDrop.StartDrag(metadata.path);
                        Event.current.Use();
                    }

                    break;
            }
        }

        public override void DrawItemBackground(Rect position, int index)
        {
            base.DrawItemBackground(position, index);

            if (this == selectedList && this[index] == selectedItem)
            {
                //GUI.DrawTexture(new RectOffset(1, 1, 1, 1).Add(position), ReorderableListStyles.SelectionBackgroundColor.GetPixel());
            }
        }

        #endregion
    }
}
