// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting.ReorderableList
{
    /// <summary>
    /// Reorderable list adaptor for serialized array property.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     This adaptor can be subclassed to add special logic to item height calculation.
    ///     You may want to implement a custom adaptor class where specialised functionality
    ///     is needed.
    ///     </para>
    ///     <para>
    ///     List elements are <b>not</b> cloned using the <see cref="System.ICloneable" />
    ///     interface when using a <see cref="UnityEditor.SerializedProperty" /> to
    ///     manipulate lists.
    ///     </para>
    /// </remarks>
    public class SerializedPropertyAdaptor : IReorderableListAdaptor
    {
        private SerializedProperty _arrayProperty;

        /// <summary>
        /// Fixed height of each list item.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Non-zero value overrides property drawer height calculation
        ///     which is more efficient.
        ///     </para>
        /// </remarks>
        public float FixedItemHeight;

        /// <summary>
        /// Gets element from list.
        /// </summary>
        /// <param name="index">Zero-based index of element.</param>
        /// <returns>
        /// Serialized property wrapper for array element.
        /// </returns>
        public SerializedProperty this[int index] => _arrayProperty.GetArrayElementAtIndex(index);

        /// <summary>
        /// Gets the underlying serialized array property.
        /// </summary>
        public SerializedProperty arrayProperty => _arrayProperty;

        #region Construction

        /// <summary>
        /// Initializes a new instance of <see cref="SerializedPropertyAdaptor" />.
        /// </summary>
        /// <param name="arrayProperty">Serialized property for entire array.</param>
        /// <param name="fixedItemHeight">Non-zero height overrides property drawer height calculation.</param>
        public SerializedPropertyAdaptor(SerializedProperty arrayProperty, float fixedItemHeight)
        {
            if (arrayProperty == null)
            {
                throw new ArgumentNullException("Array property was null.");
            }
            if (!arrayProperty.isArray)
            {
                throw new InvalidOperationException("Specified serialized propery is not an array.");
            }

            _arrayProperty = arrayProperty;
            FixedItemHeight = fixedItemHeight;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SerializedPropertyAdaptor" />.
        /// </summary>
        /// <param name="arrayProperty">Serialized property for entire array.</param>
        public SerializedPropertyAdaptor(SerializedProperty arrayProperty) : this(arrayProperty, 0f) { }

        #endregion

        #region IReorderableListAdaptor - Implementation

        /// <inheritdoc />
        public int Count => _arrayProperty.arraySize;

        /// <inheritdoc />
        public virtual bool CanDrag(int index)
        {
            return true;
        }

        /// <inheritdoc />
        public virtual bool CanRemove(int index)
        {
            return true;
        }

        /// <inheritdoc />
        public void Add()
        {
            var newIndex = _arrayProperty.arraySize;
            ++_arrayProperty.arraySize;
            Internal.SerializedPropertyUtility.ResetValue(_arrayProperty.GetArrayElementAtIndex(newIndex));
        }

        /// <inheritdoc />
        public void Insert(int index)
        {
            _arrayProperty.InsertArrayElementAtIndex(index);
            Internal.SerializedPropertyUtility.ResetValue(_arrayProperty.GetArrayElementAtIndex(index));
        }

        /// <inheritdoc />
        public void Duplicate(int index)
        {
            _arrayProperty.InsertArrayElementAtIndex(index);
        }

        /// <inheritdoc />
        public void Remove(int index)
        {
            // Unity doesn't remove element when it contains an object reference.
            var elementProperty = _arrayProperty.GetArrayElementAtIndex(index);
            if (elementProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                elementProperty.objectReferenceValue = null;
            }

            _arrayProperty.DeleteArrayElementAtIndex(index);
        }

        /// <inheritdoc />
        public void Move(int sourceIndex, int destIndex)
        {
            if (destIndex > sourceIndex)
            {
                --destIndex;
            }
            _arrayProperty.MoveArrayElement(sourceIndex, destIndex);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _arrayProperty.ClearArray();
        }

        /// <inheritdoc />
        public virtual void BeginGUI() { }

        /// <inheritdoc />
        public virtual void EndGUI() { }

        /// <inheritdoc />
        public virtual void DrawItemBackground(Rect position, int index) { }

        /// <inheritdoc />
        public virtual void DrawItem(Rect position, int index)
        {
            EditorGUI.PropertyField(position, this[index], GUIContent.none, false);
        }

        /// <inheritdoc />
        public virtual float GetItemHeight(int index)
        {
            return FixedItemHeight != 0f
                ? FixedItemHeight
                : EditorGUI.GetPropertyHeight(this[index], GUIContent.none, false)
            ;
        }

        #endregion
    }
}
