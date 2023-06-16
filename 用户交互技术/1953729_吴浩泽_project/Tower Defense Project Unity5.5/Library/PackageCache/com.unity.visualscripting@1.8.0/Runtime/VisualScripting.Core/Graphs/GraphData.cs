using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class GraphData<TGraph> : IGraphData
        where TGraph : class, IGraph
    {
        public GraphData(TGraph definition)
        {
            this.definition = definition;
        }

        protected TGraph definition { get; }

        protected Dictionary<IGraphElementWithData, IGraphElementData> elementsData { get; } = new Dictionary<IGraphElementWithData, IGraphElementData>();

        protected Dictionary<IGraphParentElement, IGraphData> childrenGraphsData { get; } = new Dictionary<IGraphParentElement, IGraphData>();

        protected Dictionary<Guid, IGraphElementData> phantomElementsData { get; } = new Dictionary<Guid, IGraphElementData>();

        protected Dictionary<Guid, IGraphData> phantomChildrenGraphsData { get; } = new Dictionary<Guid, IGraphData>();

        public bool TryGetElementData(IGraphElementWithData element, out IGraphElementData data)
        {
            return elementsData.TryGetValue(element, out data);
        }

        public bool TryGetChildGraphData(IGraphParentElement element, out IGraphData data)
        {
            return childrenGraphsData.TryGetValue(element, out data);
        }

        public IGraphElementData CreateElementData(IGraphElementWithData element)
        {
            // Debug.Log($"Creating element data for {element}");

            if (elementsData.ContainsKey(element))
            {
                throw new InvalidOperationException($"Graph data already contains element data for {element}.");
            }

            IGraphElementData elementData;

            if (phantomElementsData.TryGetValue(element.guid, out elementData))
            {
                // Debug.Log($"Restoring phantom element data for {element}.");
                phantomElementsData.Remove(element.guid);
            }
            else
            {
                elementData = element.CreateData();
            }

            elementsData.Add(element, elementData);

            return elementData;
        }

        public void FreeElementData(IGraphElementWithData element)
        {
            // Debug.Log($"Freeing element data for {element}");

            if (elementsData.TryGetValue(element, out var elementData))
            {
                elementsData.Remove(element);
                phantomElementsData.Add(element.guid, elementData);
            }
            else
            {
                Debug.LogWarning($"Graph data does not contain element data to free for {element}.");
            }
        }

        public IGraphData CreateChildGraphData(IGraphParentElement element)
        {
            // Debug.Log($"Creating child graph data for {element}");

            if (childrenGraphsData.ContainsKey(element))
            {
                throw new InvalidOperationException($"Graph data already contains child graph data for {element}.");
            }

            IGraphData childGraphData;

            if (phantomChildrenGraphsData.TryGetValue(element.guid, out childGraphData))
            {
                // Debug.Log($"Restoring phantom child graph data for {element}.");
                phantomChildrenGraphsData.Remove(element.guid);
            }
            else
            {
                childGraphData = element.childGraph.CreateData();
            }

            childrenGraphsData.Add(element, childGraphData);

            return childGraphData;
        }

        public void FreeChildGraphData(IGraphParentElement element)
        {
            // Debug.Log($"Freeing child graph data for {element}");

            if (childrenGraphsData.TryGetValue(element, out var childGraphData))
            {
                childrenGraphsData.Remove(element);
                phantomChildrenGraphsData.Add(element.guid, childGraphData);
            }
            else
            {
                Debug.LogWarning($"Graph data does not contain child graph data to free for {element}.");
            }
        }
    }
}
