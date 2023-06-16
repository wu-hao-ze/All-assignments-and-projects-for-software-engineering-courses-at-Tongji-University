using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class LudiqEditorWindow : EditorWindow, ISerializationCallbackReceiver, IHasCustomMenu
    {
        [SerializeField, DoNotSerialize] // Serialize with Unity, but not with FullSerializer.
        protected SerializationData _data;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Ignore the FullSerializer callback, but still catch the Unity callback
            if (Serialization.isCustomSerializing)
            {
                return;
            }

            Serialization.isUnitySerializing = true;

            // Starting in Unity 2018.3.0b7 apparently, the editor window tries to serialize
            // its title content and along the way, its image, which becomes an invalid reference.
            // But instead of setting it to actual null, it sets it to an invalid Unity Object,
            // which the UnityObjectConverter complains about and can't reliably detect off
            // the main thread.
            var titleImage = titleContent.image;
            titleContent.image = null;

            try
            {
                OnBeforeSerialize();
                _data = this.Serialize(true);
            }
            catch (Exception ex)
            {
                // Don't abort the whole serialization thread because this one object failed
                Debug.LogError($"Failed to serialize editor window.\n{ex}", this);
            }

            titleContent.image = titleImage;

            Serialization.isUnitySerializing = false;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Ignore the FullSerializer callback, but still catch the Unity callback
            if (Serialization.isCustomSerializing)
            {
                return;
            }

            Serialization.isUnitySerializing = true;

            try
            {
                object @this = this;
                _data.DeserializeInto(ref @this, true);
                OnAfterDeserialize();
            }
            catch (Exception ex)
            {
                // Don't abort the whole deserialization thread because this one object failed
                Debug.LogError($"Failed to deserialize editor window.\n{ex}", this);
            }

            Serialization.isUnitySerializing = false;
        }

        protected virtual void OnBeforeSerialize() { }

        protected virtual void OnAfterDeserialize() { }

        protected virtual void Update()
        {
            // Position isn't reliable in GUI calls due to layouting, so cache it here
            reliablePosition = position;
        }

        protected virtual void OnGUI()
        {
        }

        public Rect reliablePosition { get; private set; }

        public void AddItemsToMenu(GenericMenu menu)
        {
            if (_data.json != null && _data.objectReferences != null)
                menu.AddItem(new GUIContent("Show Data..."), false, () => { _data.ShowString(ToString()); });
            else
                menu.AddDisabledItem(new GUIContent("Show Data..."), false);
        }

        public override string ToString()
        {
            return this.ToSafeString();
        }
    }
}
