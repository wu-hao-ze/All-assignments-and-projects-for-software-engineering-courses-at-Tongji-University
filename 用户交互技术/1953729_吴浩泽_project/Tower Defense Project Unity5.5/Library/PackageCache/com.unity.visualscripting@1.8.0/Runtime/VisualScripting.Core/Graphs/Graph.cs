using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class Graph : IGraph
    {
        protected Graph()
        {
            elements = new MergedGraphElementCollection();
        }

        public override string ToString()
        {
            return StringUtility.FallbackWhitespace(title, base.ToString());
        }

        public abstract IGraphData CreateData();

        public virtual IGraphDebugData CreateDebugData()
        {
            return new GraphDebugData(this);
        }

        public virtual void Instantiate(GraphReference instance)
        {
            // Debug.Log($"Instantiating graph {instance}");

            foreach (var element in elements)
            {
                element.Instantiate(instance);
            }
        }

        public virtual void Uninstantiate(GraphReference instance)
        {
            // Debug.Log($"Uninstantiating graph {instance}");

            foreach (var element in elements)
            {
                element.Uninstantiate(instance);
            }
        }

        #region Elements

        [SerializeAs(nameof(elements))]
        private List<IGraphElement> _elements = new List<IGraphElement>();

        [DoNotSerialize]
        public MergedGraphElementCollection elements { get; }

        #endregion


        #region Metadata

        [Serialize]
        public string title { get; set; }

        [Serialize]
        [InspectorTextArea(minLines = 1, maxLines = 10)]
        public string summary { get; set; }

        #endregion


        #region Canvas

        [Serialize]
        public Vector2 pan { get; set; }

        [Serialize]
        public float zoom { get; set; } = 1;

        #endregion


        #region Serialization

        public IEnumerable<ISerializationDependency> deserializationDependencies => _elements.SelectMany(e => e.deserializationDependencies);

        public virtual void OnBeforeSerialize()
        {
            _elements.Clear();
            _elements.AddRange(elements);
        }

        public void OnAfterDeserialize()
        {
            Serialization.AwaitDependencies(this);
        }

        public virtual void OnAfterDependenciesDeserialized()
        {
            elements.Clear();

            // _elements.OrderBy(e => e.dependencyOrder)
            var sortedElements = ListPool<IGraphElement>.New();
            foreach (var element in _elements)
            {
                sortedElements.Add(element);
            }
            sortedElements.Sort((a, b) => a.dependencyOrder.CompareTo(b.dependencyOrder));

            foreach (var element in sortedElements)
            {
                try
                {
                    if (!element.HandleDependencies())
                    {
                        continue;
                    }

                    elements.Add(element);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to add element to graph during deserialization: {element}\n{ex}");
                }
            }

            ListPool<IGraphElement>.Free(sortedElements);
        }

        #endregion


        #region Poutine

        public IEnumerable<object> GetAotStubs(HashSet<object> visited)
        {
            return elements
                .Where(element => !visited.Contains(element))
                .Select(element =>
                {
                    visited.Add(element);
                    return element;
                })
                .SelectMany(element => element.GetAotStubs(visited));
        }

        private bool prewarmed;

        public void Prewarm()
        {
            if (prewarmed)
            {
                return;
            }

            foreach (var element in elements)
            {
                element.Prewarm();
            }

            prewarmed = true;
        }

        public virtual void Dispose()
        {
            foreach (var element in elements)
            {
                element.Dispose();
            }
        }

        #endregion
    }
}
