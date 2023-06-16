using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Timeline
{
    struct TrackItemsDrawer
    {
        List<ILayer> m_Layers;
        ClipsLayer m_ClipsLayer;

        public List<TimelineClipGUI> clips => m_ClipsLayer.items;

        public TrackItemsDrawer(IRowGUI parent)
        {
            m_Layers = null;
            m_ClipsLayer = null;
            BuildGUICache(parent);
        }

        void BuildGUICache(IRowGUI parent)
        {
            m_ClipsLayer = new ClipsLayer(Layer.Clips, parent);
            m_Layers = new List<ILayer>
            {
                m_ClipsLayer,
                new MarkersLayer(Layer.Markers, parent)
            };
        }

        public void Draw(Rect rect, WindowState state)
        {
            foreach (var layer in m_Layers)
            {
                layer.Draw(rect, state);
            }
        }
    }
}
