using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    enum Layer : byte
    {
        Clips,
        ClipHandles,
        Markers,
        MarkerHeaderTrack,
        MarkersOnHeader
    }

    struct LayerZOrder : IComparable<LayerZOrder>
    {
        Layer m_Layer;
        int m_ZOrder;

        public LayerZOrder(Layer layer, int zOrder)
        {
            m_Layer = layer;
            m_ZOrder = zOrder;
        }

        public int CompareTo(LayerZOrder other)
        {
            if (m_Layer == other.m_Layer)
                return m_ZOrder.CompareTo(other.m_ZOrder);
            return m_Layer.CompareTo(other.m_Layer);
        }

        public static LayerZOrder operator ++(LayerZOrder x)
        {
            return new LayerZOrder(x.m_Layer, x.m_ZOrder + 1);
        }

        public LayerZOrder ChangeLayer(Layer layer)
        {
            return new LayerZOrder(layer, m_ZOrder);
        }
    }

    interface ILayerable
    {
        LayerZOrder zOrder { get; }
    }

    interface IZOrderProvider
    {
        LayerZOrder Next();
    }

    interface ILayer : IZOrderProvider
    {
        void Draw(Rect rect, WindowState state);
    }

    abstract class ItemsLayer<T> : ILayer where T : TimelineItemGUI
    {
        // provide a buffer for time-based culling to allow for UI that extends slightly beyond the time (e.g. markers)
        // prevents popping of marker visibility.
        const int kVisibilityBufferInPixels = 10;

        int m_PreviousLayerStateHash = -1;
        LayerZOrder m_LastZOrder;

        public LayerZOrder Next()
        {
            m_NeedSort = true;
            return m_LastZOrder++;
        }

        readonly List<T> m_Items = new List<T>();
        bool m_NeedSort = true;

        public virtual void Draw(Rect rect, WindowState state)
        {
            if (m_Items.Count <= 0) return;

            Sort();

            // buffer to prevent flickering of markers at boundaries
            var onePixelTime = state.PixelDeltaToDeltaTime(kVisibilityBufferInPixels);
            var visibleTime = state.timeAreaShownRange + new Vector2(-onePixelTime, onePixelTime);
            var layerViewStateHasChanged = GetLayerViewStateChanged(rect, state);

            foreach (var item in m_Items)
            {
                item.visible = item.end > visibleTime.x && item.start < visibleTime.y;
                if (!item.visible)
                    continue;

                item.Draw(rect, layerViewStateHasChanged, state);
            }
        }

        public List<T> items => m_Items;

        protected void AddItem(T item)
        {
            m_Items.Add(item);
            m_NeedSort = true;
        }

        protected ItemsLayer(Layer layerOrder)
        {
            m_LastZOrder = new LayerZOrder(layerOrder, 0);
        }

        void Sort()
        {
            if (!m_NeedSort)
                return;

            m_Items.Sort((a, b) => a.zOrder.CompareTo(b.zOrder));
            m_NeedSort = false;
        }

        bool GetLayerViewStateChanged(Rect rect, WindowState state)
        {
            var layerStateHash = rect.GetHashCode().CombineHash(state.viewStateHash);
            var layerViewStateHasChanged = layerStateHash != m_PreviousLayerStateHash;

            if (Event.current.type == EventType.Layout && layerViewStateHasChanged)
                m_PreviousLayerStateHash = layerStateHash;

            return layerViewStateHasChanged;
        }
    }
}
