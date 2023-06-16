using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor.Timeline
{
    class CurveTreeViewNode : TreeViewItem
    {
        public bool forceGroup { get; }
        public System.Type iconType { get; }
        public GUIContent iconOverlay { get; }

        EditorCurveBinding[] m_Bindings;

        public EditorCurveBinding[] bindings
        {
            get { return m_Bindings; }
        }

        public CurveTreeViewNode(int id, TreeViewItem parent, string displayName, EditorCurveBinding[] bindings, bool _forceGroup = false)
            : base(id, parent != null ? parent.depth + 1 : -1, parent, displayName)
        {
            m_Bindings = bindings;
            forceGroup = _forceGroup;


            // capture the preview icon type. If all subbindings are the same type, use that. Otherwise use null as a default
            iconType = null;
            if (parent != null && parent.depth >= 0 && bindings != null && bindings.Length > 0 && bindings.All(b => b.type == bindings[0].type))
            {
                iconType = bindings[0].type;

                // for components put the component type in a tooltip
                if (iconType != null && typeof(Component).IsAssignableFrom(iconType))
                    iconOverlay = new GUIContent(string.Empty, ObjectNames.NicifyVariableName(iconType.Name));
            }
        }
    }
}
