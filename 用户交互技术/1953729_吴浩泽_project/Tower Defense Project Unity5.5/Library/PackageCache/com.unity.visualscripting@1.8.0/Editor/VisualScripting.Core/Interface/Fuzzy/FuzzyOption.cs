using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class FuzzyOption<T> : IFuzzyOption
    {
        public T value { get; protected set; }

        object IFuzzyOption.value => value;

        public string label { get; protected set; }

        public EditorTexture icon { get; protected set; }

        public GUIStyle style { get; protected set; }

        public virtual string headerLabel => label;

        public bool parentOnly { get; protected set; }

        public bool showHeaderIcon { get; protected set; }

        public virtual bool hasFooter { get; protected set; }

        public virtual float GetFooterHeight(float width)
        {
            return 0;
        }

        public virtual void OnFooterGUI(Rect position) { }

        public virtual void OnPopulate() { }
    }
}
