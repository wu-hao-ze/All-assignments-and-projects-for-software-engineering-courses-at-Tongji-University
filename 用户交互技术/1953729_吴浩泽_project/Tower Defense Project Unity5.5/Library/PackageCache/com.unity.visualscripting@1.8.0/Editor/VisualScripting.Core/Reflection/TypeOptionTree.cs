using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class TypeOptionTree : FuzzyOptionTree
    {
        public enum RootMode
        {
            Types,
            Namespaces
        }

        private TypeOptionTree() : base(new GUIContent("Type")) { }

        public override IFuzzyOption Option(object item)
        {
            if (item is Namespace)
            {
                return new NamespaceOption((Namespace)item, true);
            }

            return base.Option(item);
        }

        public TypeOptionTree(IEnumerable<Type> types) : this()
        {
            Ensure.That(nameof(types)).IsNotNull(types);
            this.types = types.ToHashSet();
        }

        public TypeOptionTree(IEnumerable<Type> typeSet, TypeFilter filter) : this()
        {
            Ensure.That(nameof(typeSet)).IsNotNull(typeSet);
            Ensure.That(nameof(filter)).IsNotNull(filter);
            this.typeSet = typeSet;
            this.filter = filter;
        }

        private readonly IEnumerable<Type> typeSet;

        private readonly TypeFilter filter;

        private HashSet<Type> types;

        public RootMode rootMode { get; set; } = RootMode.Namespaces;

        public override void Prewarm()
        {
            base.Prewarm();

            types ??= typeSet.Concat(typeSet.Where(t => !t.IsStatic()).Select(t => typeof(List<>).MakeGenericType(t))) // Add lists
                .Where(filter.Configured().ValidateType) // Filter
                .ToHashSet();

            groupEnums = !types.All(t => t.IsEnum);
        }

        #region Configuration

        public bool groupEnums { get; set; } = true;

        public bool surfaceCommonTypes { get; set; } = true;

        #endregion

        #region Hierarchy

        private readonly FuzzyGroup enumsGroup = new FuzzyGroup("(Enums)", typeof(Enum).Icon());

        public override IEnumerable<object> Root()
        {
            if (rootMode == RootMode.Namespaces)
            {
                if (surfaceCommonTypes)
                {
                    foreach (var type in EditorTypeUtility.commonTypes)
                    {
                        if (types.Contains(type))
                        {
                            yield return type;
                        }
                    }
                }

                foreach (var @namespace in types.Where(t => !(groupEnums && t.IsEnum))
                         .Select(t => t.Namespace().Root)
                         .Distinct()
                         .OrderBy(ns => ns.DisplayName(false))
                         .Cast<object>())
                {
                    yield return @namespace;
                }

                if (groupEnums && types.Any(t => t.IsEnum))
                {
                    yield return enumsGroup;
                }
            }
            else if (rootMode == RootMode.Types)
            {
                foreach (var type in types)
                {
                    yield return type;
                }
            }
            else
            {
                throw new UnexpectedEnumValueException<RootMode>(rootMode);
            }
        }

        public override IEnumerable<object> Children(object parent)
        {
            if (parent is Namespace)
            {
                var @namespace = (Namespace)parent;

                if (!@namespace.IsGlobal)
                {
                    foreach (var childNamespace in types.Where(t => !(groupEnums && t.IsEnum))
                             .SelectMany(t => t.Namespace().AndAncestors())
                             .Distinct()
                             .Where(ns => ns.Parent == @namespace)
                             .OrderBy(ns => ns.DisplayName(false)))
                    {
                        yield return childNamespace;
                    }
                }

                foreach (var type in types.Where(t => t.Namespace() == @namespace)
                         .Where(t => !(groupEnums && t.IsEnum))
                         .OrderBy(t => t.DisplayName()))
                {
                    yield return type;
                }
            }
            else if (parent == enumsGroup)
            {
                foreach (var type in types.Where(t => t.IsEnum)
                         .OrderBy(t => t.DisplayName()))
                {
                    yield return type;
                }
            }
        }

        #endregion

        #region Search

        public override bool searchable { get; } = true;

        public override IEnumerable<ISearchResult> SearchResults(string query, CancellationToken cancellation)
        {
            return types.OrderableSearchFilter(query, TypeOption.Haystack).Cast<ISearchResult>();
        }

        public override string SearchResultLabel(object item, string query)
        {
            return TypeOption.SearchResultLabel((Type)item, query);
        }

        #endregion
    }
}
