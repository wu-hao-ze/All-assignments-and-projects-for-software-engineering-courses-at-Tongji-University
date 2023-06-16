using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class GraphInstances
    {
        private static readonly object @lock = new object();

        private static readonly Dictionary<IGraph, HashSet<GraphReference>> byGraph = new Dictionary<IGraph, HashSet<GraphReference>>();

        private static readonly Dictionary<IGraphParent, HashSet<GraphReference>> byParent = new Dictionary<IGraphParent, HashSet<GraphReference>>();

        public static void Instantiate(GraphReference instance)
        {
            lock (@lock)
            {
                Ensure.That(nameof(instance)).IsNotNull(instance);

                instance.CreateGraphData();

                instance.graph.Instantiate(instance);

                if (!byGraph.TryGetValue(instance.graph, out var instancesWithGraph))
                {
                    instancesWithGraph = new HashSet<GraphReference>();
                    byGraph.Add(instance.graph, instancesWithGraph);
                }

                if (instancesWithGraph.Add(instance))
                {
                    // Debug.Log($"Added graph instance mapping:\n{instance.graph} => {instance}");
                }
                else
                {
                    Debug.LogWarning($"Attempting to add duplicate graph instance mapping:\n{instance.graph} => {instance}");
                }

                if (!byParent.TryGetValue(instance.parent, out var instancesWithParent))
                {
                    instancesWithParent = new HashSet<GraphReference>();
                    byParent.Add(instance.parent, instancesWithParent);
                }

                if (instancesWithParent.Add(instance))
                {
                    // Debug.Log($"Added parent instance mapping:\n{instance.parent.ToSafeString()} => {instance}");
                }
                else
                {
                    Debug.LogWarning($"Attempting to add duplicate parent instance mapping:\n{instance.parent.ToSafeString()} => {instance}");
                }
            }
        }

        public static void Uninstantiate(GraphReference instance)
        {
            lock (@lock)
            {
                instance.graph.Uninstantiate(instance);

                if (!byGraph.TryGetValue(instance.graph, out var instancesWithGraph))
                {
                    throw new InvalidOperationException("Graph instance not found via graph.");
                }

                if (instancesWithGraph.Remove(instance))
                {
                    // Debug.Log($"Removed graph instance mapping:\n{instance.graph} => {instance}");

                    // Free the key references for GC collection
                    if (instancesWithGraph.Count == 0)
                    {
                        byGraph.Remove(instance.graph);
                    }
                }
                else
                {
                    Debug.LogWarning($"Could not find graph instance mapping to remove:\n{instance.graph} => {instance}");
                }

                if (!byParent.TryGetValue(instance.parent, out var instancesWithParent))
                {
                    throw new InvalidOperationException("Graph instance not found via parent.");
                }

                if (instancesWithParent.Remove(instance))
                {
                    // Debug.Log($"Removed parent instance mapping:\n{instance.parent.ToSafeString()} => {instance}");

                    // Free the key references for GC collection
                    if (instancesWithParent.Count == 0)
                    {
                        byParent.Remove(instance.parent);
                    }
                }
                else
                {
                    Debug.LogWarning($"Could not find parent instance mapping to remove:\n{instance.parent.ToSafeString()} => {instance}");
                }

                // It's important to only free the graph data after
                // dissociating the instance mapping, because the data
                // is used as part of the equality comparison for pointers
                instance.FreeGraphData();
            }
        }

        public static HashSet<GraphReference> OfPooled(IGraph graph)
        {
            Ensure.That(nameof(graph)).IsNotNull(graph);

            lock (@lock)
            {
                if (byGraph.TryGetValue(graph, out var instances))
                {
                    // Debug.Log($"Found {instances.Count} instances of {graph}\n{instances.ToLineSeparatedString()}");

                    return instances.ToHashSetPooled();
                }
                else
                {
                    // Debug.Log($"Found no instances of {graph}.\n");

                    return HashSetPool<GraphReference>.New();
                }
            }
        }

        public static HashSet<GraphReference> ChildrenOfPooled(IGraphParent parent)
        {
            Ensure.That(nameof(parent)).IsNotNull(parent);

            lock (@lock)
            {
                if (byParent.TryGetValue(parent, out var instances))
                {
                    // Debug.Log($"Found {instances.Count} instances of {parent.ToSafeString()}\n{instances.ToLineSeparatedString()}");

                    return instances.ToHashSetPooled();
                }
                else
                {
                    // Debug.Log($"Found no instances of {parent.ToSafeString()}.\n");

                    return HashSetPool<GraphReference>.New();
                }
            }
        }
    }
}
