using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Timeline;

#if !UNITY_2020_2_OR_NEWER
using L10n = UnityEditor.Timeline.L10n;
#endif

namespace UnityEditor.Timeline
{
    class BindingTreeViewDataSource : TreeViewDataSource
    {
        struct BindingGroup : IEquatable<BindingGroup>, IComparable<BindingGroup>
        {
            public readonly string GroupName;
            public readonly string Path;
            public readonly Type Type;

            public BindingGroup(string path, string groupName, Type type)
            {
                Path = path;
                GroupName = groupName;
                Type = type;
            }

            public string groupDisplayName => string.IsNullOrEmpty(Path) ? GroupName : string.Format($"{Path} : {GroupName}");
            public bool Equals(BindingGroup other) => GroupName == other.GroupName && Type == other.Type && Path == other.Path;
            public int CompareTo(BindingGroup other) => GetHashCode() - other.GetHashCode();
            public override bool Equals(object obj) => obj is BindingGroup other && Equals(other);
            public override int GetHashCode()
            {
                return HashUtility.CombineHash(GroupName != null ? GroupName.GetHashCode() : 0, Type != null ? Type.GetHashCode() : 0, Path != null ? Path.GetHashCode() : 0);
            }
        }

        static readonly string s_DefaultValue = L10n.Tr("{0} (Default Value)");

        public const int RootID = int.MinValue;
        public const int GroupID = -1;

        private readonly AnimationClip m_Clip;
        private readonly CurveDataSource m_CurveDataSource;

        public BindingTreeViewDataSource(
            TreeViewController treeView, AnimationClip clip, CurveDataSource curveDataSource)
            : base(treeView)
        {
            m_Clip = clip;
            showRootItem = false;
            m_CurveDataSource = curveDataSource;
        }

        void SetupRootNodeSettings()
        {
            showRootItem = false;
            SetExpanded(RootID, true);
            SetExpanded(GroupID, true);
        }

        public override void FetchData()
        {
            if (m_Clip == null)
                return;

            var bindings = AnimationUtility.GetCurveBindings(m_Clip)
                .Union(AnimationUtility.GetObjectReferenceCurveBindings(m_Clip))
                .ToArray();

            // a sorted linear list of nodes
            var results = bindings.GroupBy(GetBindingGroup, p => p, CreateTuple)
                .OrderBy(t => t.Item1.Path)
                .ThenBy(NamePrioritySort)
                // this makes component ordering match the animation window
                .ThenBy(t => t.Item1.Type.ToString())
                .ThenBy(t => t.Item1.GroupName).ToArray();

            m_RootItem = new CurveTreeViewNode(RootID, null, "root", null)
            {
                children = new List<TreeViewItem>(1)
            };

            if (results.Any())
            {
                var groupingNode = new CurveTreeViewNode(GroupID, m_RootItem, m_CurveDataSource.groupingName, bindings)
                {
                    children = new List<TreeViewItem>()
                };

                m_RootItem.children.Add(groupingNode);

                foreach (var r in results)
                {
                    var key = r.Item1;
                    var nodeBindings = r.Item2;

                    FillMissingTransformCurves(nodeBindings);
                    if (nodeBindings.Count == 1)
                        groupingNode.children.Add(CreateLeafNode(nodeBindings[0], groupingNode, PropertyName(nodeBindings[0], true)));
                    else if (nodeBindings.Count > 1)
                    {
                        var childBindings = nodeBindings.OrderBy(BindingSort).ToArray();
                        var parent = new CurveTreeViewNode(key.GetHashCode(), groupingNode, key.groupDisplayName, childBindings) { children = new List<TreeViewItem>() };
                        groupingNode.children.Add(parent);
                        foreach (var b in childBindings)
                            parent.children.Add(CreateLeafNode(b, parent, PropertyName(b, false)));
                    }
                }
                SetupRootNodeSettings();
            }

            m_NeedRefreshRows = true;
        }

        public void UpdateData()
        {
            m_TreeView.ReloadData();
        }

        string GroupName(EditorCurveBinding binding)
        {
            var propertyName = m_CurveDataSource.ModifyPropertyDisplayName(binding.path, binding.propertyName);
            return CleanUpArrayBinding(AnimationWindowUtility.NicifyPropertyGroupName(binding.type, propertyName), true);
        }

        static string CleanUpArrayBinding(string propertyName, bool isGroup)
        {
            const string arrayIndicator = ".Array.data[";
            const string arrayDisplay = ".data[";

            var arrayIndex = propertyName.LastIndexOf(arrayIndicator, StringComparison.Ordinal);
            if (arrayIndex == -1)
                return propertyName;
            if (isGroup)
                propertyName = propertyName.Substring(0, arrayIndex);
            return propertyName.Replace(arrayIndicator, arrayDisplay);
        }

        string PropertyName(EditorCurveBinding binding, bool prependPathName)
        {
            var propertyName = m_CurveDataSource.ModifyPropertyDisplayName(binding.path, binding.propertyName);
            propertyName = CleanUpArrayBinding(AnimationWindowUtility.GetPropertyDisplayName(propertyName), false);
            if (binding.isPhantom)
                propertyName = string.Format(s_DefaultValue, propertyName);
            if (prependPathName && !string.IsNullOrEmpty(binding.path))
                propertyName = $"{binding.path} : {propertyName}";
            return propertyName;
        }

        BindingGroup GetBindingGroup(EditorCurveBinding binding)
        {
            return new BindingGroup(binding.path ?? string.Empty, GroupName(binding), binding.type);
        }

        static CurveTreeViewNode CreateLeafNode(EditorCurveBinding binding, TreeViewItem parent, string displayName)
        {
            return new CurveTreeViewNode(binding.GetHashCode(), parent, displayName, new[] { binding }, AnimationWindowUtility.ForceGrouping(binding));
        }

        static void FillMissingTransformCurves(List<EditorCurveBinding> bindings)
        {
            if (!AnimationWindowUtility.IsActualTransformCurve(bindings[0]) || bindings.Count >= 3)
                return;

            var binding = bindings[0];
            var prefixPropertyName = binding.propertyName.Split('.').First();

            binding.isPhantom = true;
            if (!bindings.Any(p => p.propertyName.EndsWith(".x")))
            {
                binding.propertyName = prefixPropertyName + ".x";
                bindings.Insert(0, binding);
            }

            if (!bindings.Any(p => p.propertyName.EndsWith(".y")))
            {
                binding.propertyName = prefixPropertyName + ".y";
                bindings.Insert(1, binding);
            }

            if (!bindings.Any(p => p.propertyName.EndsWith(".z")))
            {
                binding.propertyName = prefixPropertyName + ".z";
                bindings.Insert(2, binding);
            }
        }

        // make sure vectors and colors are sorted correctly in their subgroups
        static int BindingSort(EditorCurveBinding b)
        {
            return AnimationWindowUtility.GetComponentIndex(b.propertyName);
        }

        static int NamePrioritySort(ValueTuple<BindingGroup, List<EditorCurveBinding>> group)
        {
            if (group.Item1.Type != typeof(Transform))
                return 0;

            switch (group.Item1.GroupName)
            {
                case "Position": return Int32.MinValue;
                case "Rotation": return Int32.MinValue + 1;
                case "Scale": return Int32.MinValue + 2;
                default: return 0;
            }
        }

        static ValueTuple<BindingGroup, List<EditorCurveBinding>> CreateTuple(BindingGroup key, IEnumerable<EditorCurveBinding> items)
        {
            return new ValueTuple<BindingGroup, List<EditorCurveBinding>>()
            {
                Item1 = key,
                Item2 = items.ToList()
            };
        }
    }
}
