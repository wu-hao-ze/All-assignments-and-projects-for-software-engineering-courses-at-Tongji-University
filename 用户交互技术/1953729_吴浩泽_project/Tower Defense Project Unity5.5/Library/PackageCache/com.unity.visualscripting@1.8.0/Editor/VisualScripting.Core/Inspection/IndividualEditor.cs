using System;
using UnityEditor;

namespace Unity.VisualScripting
{
    public abstract class IndividualEditor : IDisposable
    {
        public void Initialize(SerializedObject serializedObject, Editor editorParent)
        {
            this.serializedObject = serializedObject;
            this.editorParent = editorParent;
            Initialize();
        }

        protected abstract void Initialize();
        public Editor editorParent { get; private set; }
        public SerializedObject serializedObject { get; private set; }

        public abstract void OnGUI();

        public virtual void Dispose() { }
    }
}
