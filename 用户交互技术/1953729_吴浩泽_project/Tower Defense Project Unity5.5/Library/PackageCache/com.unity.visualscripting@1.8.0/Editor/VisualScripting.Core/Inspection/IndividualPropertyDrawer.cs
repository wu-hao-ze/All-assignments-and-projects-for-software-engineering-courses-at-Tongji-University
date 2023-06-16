using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class IndividualPropertyDrawer : IDisposable
    {
        public void Initialize(SerializedProperty property)
        {
            this.property = property;
            Initialize();
        }

        protected abstract void Initialize();
        public SerializedProperty property { get; private set; }

        public abstract float GetHeight(GUIContent label);

        public abstract void OnGUI(Rect drawerPosition, GUIContent label);

        public virtual void Dispose() { }
    }
}
