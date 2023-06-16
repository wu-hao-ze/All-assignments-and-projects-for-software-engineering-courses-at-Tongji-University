using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class FuzzyOptionNode
    {
        public FuzzyOptionNode(IFuzzyOption option)
        {
            Ensure.That(nameof(option)).IsNotNull(option);

            this.option = option;
            children = new List<FuzzyOptionNode>();
            labelText = option.label;
        }

        public FuzzyOptionNode(IFuzzyOption option, string label)
        {
            Ensure.That(nameof(option)).IsNotNull(option);
            Ensure.That(nameof(label)).IsNotNull(label);

            this.option = option;
            children = new List<FuzzyOptionNode>();
            labelText = label;
        }

        #region Data

        public IFuzzyOption option { get; }
        public string labelText { get; private set; }
        public List<FuzzyOptionNode> children { get; }
        public bool hasChildren { get; set; }
        public bool isPopulated { get; set; }
        public bool isLoading { get; set; } = true;

        #endregion

        #region Interaction

        public Vector2 scroll { get; set; }
        public int selectedIndex { get; set; }

        #endregion

        #region Drawing

        public bool isDrawable { get; private set; }
        public GUIContent label { get; private set; }
        public GUIStyle style { get; private set; }
        public float width { get; private set; }

        public void EnsureDrawable()
        {
            if (!isDrawable)
            {
                PrepareDrawing();
            }
        }

        public void PrepareDrawing()
        {
            if (isDrawable)
            {
                return;
            }

            label = new GUIContent(labelText, option.icon?[IconSize.Small]);
            style = option.style ?? (option.icon != null ? FuzzyWindow.Styles.optionWithIcon : FuzzyWindow.Styles.optionWithoutIcon);
            width = style.CalcSize(label).x;
            isDrawable = true;
        }

        #endregion
    }
}
