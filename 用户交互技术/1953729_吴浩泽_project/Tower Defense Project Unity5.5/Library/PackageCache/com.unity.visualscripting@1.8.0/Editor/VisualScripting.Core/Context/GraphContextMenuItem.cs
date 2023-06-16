using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    public struct GraphContextMenuItem
    {
        public Action<Vector2> action { get; }
        public string label { get; }

        public GraphContextMenuItem(Action<Vector2> action, string label)
        {
            this.action = action;
            this.label = label;
        }
    }
}
