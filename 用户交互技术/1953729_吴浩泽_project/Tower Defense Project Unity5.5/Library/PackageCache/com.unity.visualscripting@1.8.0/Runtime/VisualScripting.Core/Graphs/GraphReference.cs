using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public sealed class GraphReference : GraphPointer
    {
        static GraphReference()
        {
            ReferenceCollector.onSceneUnloaded += FreeInvalidInterns;
        }

        #region Lifecycle

        private GraphReference() { }

        public static GraphReference New(IGraphRoot root, bool ensureValid)
        {
            if (!ensureValid && !IsValidRoot(root))
            {
                return null;
            }

            var reference = new GraphReference();
            reference.Initialize(root);
            reference.Hash();
            return reference;
        }

        public static GraphReference New(IGraphRoot root, IEnumerable<IGraphParentElement> parentElements, bool ensureValid)
        {
            if (!ensureValid && !IsValidRoot(root))
            {
                return null;
            }

            var reference = new GraphReference();
            reference.Initialize(root, parentElements, ensureValid);
            reference.Hash();
            return reference;
        }

        public static GraphReference New(UnityObject rootObject, IEnumerable<Guid> parentElementGuids, bool ensureValid)
        {
            if (!ensureValid && !IsValidRoot(rootObject))
            {
                return null;
            }

            var reference = new GraphReference();
            reference.Initialize(rootObject, parentElementGuids, ensureValid);
            reference.Hash();
            return reference;
        }

        private static GraphReference New(GraphPointer model)
        {
            var reference = new GraphReference();
            reference.CopyFrom(model);
            return reference;
        }

        public override void CopyFrom(GraphPointer other)
        {
            base.CopyFrom(other);

            if (other is GraphReference reference)
            {
                hashCode = reference.hashCode;
            }
            else
            {
                Hash();
            }
        }

        public GraphReference Clone()
        {
            return New(this);
        }

        #endregion


        #region Conversion

        public override GraphReference AsReference()
        {
            return this;
        }

        public GraphStack ToStackPooled()
        {
            return GraphStack.New(this);
        }

        #endregion


        #region Instantiation

        public void CreateGraphData()
        {
            if (_data != null)
            {
                throw new GraphPointerException("Graph data already exists.", this);
            }

            if (isRoot)
            {
                if (machine != null)
                {
                    // Debug.Log($"Creating root graph data for {this}");

                    _data = machine.graphData = graph.CreateData();
                }
                else
                {
                    throw new GraphPointerException("Root graph data can only be created on machines.", this);
                }
            }
            else
            {
                if (_parentData == null)
                {
                    throw new GraphPointerException("Child graph data can only be created from parent graph data.", this);
                }

                _data = _parentData.CreateChildGraphData(parentElement);
            }
        }

        public void FreeGraphData()
        {
            if (_data == null)
            {
                throw new GraphPointerException("Graph data does not exist.", this);
            }

            if (isRoot)
            {
                if (machine != null)
                {
                    // Debug.Log($"Freeing root graph data for {this}");

                    _data = machine.graphData = null;
                }
                else
                {
                    throw new GraphPointerException("Root graph data can only be freed on machines.", this);
                }
            }
            else
            {
                if (_parentData == null)
                {
                    throw new GraphPointerException("Child graph data can only be freed from parent graph data.", this);
                }

                _parentData.FreeChildGraphData(parentElement);
                _data = null;
            }
        }

        #endregion


        #region Equality

        [DoNotSerialize]
        private int hashCode;

        public override bool Equals(object obj)
        {
            if (!(obj is GraphReference other))
            {
                return false;
            }

            return InstanceEquals(other);
        }

        private void Hash()
        {
            hashCode = ComputeHashCode();
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public static bool operator ==(GraphReference x, GraphReference y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(GraphReference x, GraphReference y)
        {
            return !(x == y);
        }

        #endregion


        #region Traversal

        public GraphReference ParentReference(bool ensureValid)
        {
            if (isRoot)
            {
                if (ensureValid)
                {
                    throw new GraphPointerException("Trying to get parent graph reference of a root.", this);
                }
                else
                {
                    return null;
                }
            }

            var pointer = Clone();
            pointer.ExitParentElement();
            pointer.Hash();
            return pointer;
        }

        public GraphReference ChildReference(IGraphParentElement parentElement, bool ensureValid, int? maxRecursionDepth = null)
        {
            var pointer = Clone();

            if (!pointer.TryEnterParentElement(parentElement, out var error, maxRecursionDepth))
            {
                if (ensureValid)
                {
                    throw new GraphPointerException(error, this);
                }
                else
                {
                    return null;
                }
            }

            pointer.Hash();
            return pointer;
        }

        #endregion


        #region Validation

        public GraphReference Revalidate(bool ensureValid)
        {
            try
            {
                // Important to recreate by GUIDs to avoid serialization ghosting
                return New(rootObject, parentElementGuids, ensureValid);
            }
            catch (Exception ex)
            {
                if (ensureValid)
                {
                    throw;
                }

                Debug.LogWarning("Failed to revalidate graph pointer: \n" + ex);
                return null;
            }
        }

        #endregion


        #region Navigation

        public IEnumerable<GraphReference> GetBreadcrumbs()
        {
            for (int depth = 0; depth < this.depth; depth++)
            {
                yield return New(root, parentElementStack.Take(depth), true);
            }
        }

        #endregion


        #region Interning

        private static readonly Dictionary<int, List<GraphReference>> internPool = new Dictionary<int, List<GraphReference>>();

        public static GraphReference Intern(GraphPointer pointer)
        {
            var hash = pointer.ComputeHashCode();

            if (internPool.TryGetValue(hash, out var interns))
            {
                foreach (var intern in interns)
                {
                    if (intern.InstanceEquals(pointer))
                    {
                        return intern;
                    }
                }

                var reference = New(pointer);
                interns.Add(reference);
                return reference;
            }
            else
            {
                var reference = New(pointer);
                internPool.Add(reference.hashCode, new List<GraphReference>() { reference });
                return reference;
            }
        }

        public static void FreeInvalidInterns()
        {
            var invalidHashes = ListPool<int>.New();

            foreach (var internsByHash in internPool)
            {
                var hash = internsByHash.Key;
                var interns = internsByHash.Value;

                var invalidInterns = ListPool<GraphReference>.New();

                foreach (var intern in interns)
                {
                    if (!intern.isValid)
                    {
                        invalidInterns.Add(intern);
                    }
                }

                foreach (var intern in invalidInterns)
                {
                    interns.Remove(intern);
                }

                if (interns.Count == 0)
                {
                    invalidHashes.Add(hash);
                }

                invalidInterns.Free();
            }

            foreach (var hash in invalidHashes)
            {
                internPool.Remove(hash);
            }

            invalidHashes.Free();
        }

        #endregion
    }
}
