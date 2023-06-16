using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public class MemberOptionTree : FuzzyOptionTree
    {
        public enum RootMode
        {
            Members,
            Types,
            Namespaces
        }

        public MemberOptionTree(IEnumerable<Type> types, MemberFilter memberFilter, TypeFilter memberTypeFilter, ActionDirection direction) : base(new GUIContent("Member"))
        {
            favorites = new Favorites(this);
            codebase = Codebase.Subset(types, memberFilter.Configured(), memberTypeFilter?.Configured(false));
            this.direction = direction;
            expectingBoolean = memberTypeFilter?.ExpectsBoolean ?? false;
        }

        public MemberOptionTree(UnityObject target, MemberFilter memberFilter, TypeFilter memberTypeFilter, ActionDirection direction)
            : this(EditorUnityObjectUtility.GetUnityTypes(target), memberFilter, memberTypeFilter, direction)
        {
            rootMode = RootMode.Types;
        }

        private readonly CodebaseSubset codebase;

        private readonly ActionDirection direction;

        private readonly bool expectingBoolean;

        private readonly RootMode rootMode = RootMode.Namespaces;

        public override void Prewarm()
        {
            base.Prewarm();

            codebase.Cache();
        }

        public override IFuzzyOption Option(object item)
        {
            if (item is Member)
            {
                return new MemberOption((Member)item, direction, expectingBoolean);
            }

            if (item is Type)
            {
                return new TypeOption((Type)item, true);
            }

            return base.Option(item);
        }

        #region Hierarchy

        public override IEnumerable<object> Root()
        {
            if (rootMode == RootMode.Types)
            {
                foreach (var type in codebase.members
                         .Select(m => m.targetType)
                         .Distinct()
                         .OrderBy(t => t.DisplayName()))
                {
                    yield return type;
                }
            }
            else if (rootMode == RootMode.Namespaces)
            {
                foreach (var @namespace in codebase.members.Select(m => m.targetType)
                         .Distinct()
                         .Where(t => !t.IsEnum)
                         .Select(t => t.Namespace().Root)
                         .Distinct()
                         .OrderBy(ns => ns.DisplayName(false)))
                {
                    yield return @namespace;
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
                    foreach (var childNamespace in codebase.members
                             .Select(m => m.targetType)
                             .Distinct()
                             .Where(t => !t.IsEnum)
                             .Select(t => t.Namespace())
                             .Distinct()
                             .Where(ns => ns.Parent == @namespace)
                             .OrderBy(ns => ns.DisplayName(false)))
                    {
                        yield return childNamespace;
                    }
                }

                foreach (var type in codebase.members
                         .Select(m => m.targetType)
                         .Where(t => !t.IsEnum)
                         .Distinct()
                         .Where(t => t.Namespace() == @namespace)
                         .OrderBy(t => t.DisplayName()))
                {
                    yield return type;
                }
            }
            else if (parent is Type)
            {
                foreach (var member in codebase.members
                         .Where(m => m.targetType == (Type)parent)
                         .OrderBy(m => BoltCore.Configuration.groupInheritedMembers && m.isPseudoInherited)
                         .ThenBy(m => m.info.DisplayName()))
                {
                    yield return member;
                }
            }
            else if (parent is Member)
            {
                yield break;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Search

        public override bool searchable { get; } = true;

        public override IEnumerable<object> OrderedSearchResults(string query, CancellationToken cancellation)
        {
            // Exclude duplicate inherited members, like the high amount of "Destroy()" or "enabled",
            // if their declaring type is also available for search.

            foreach (var member in codebase.members
                     .Cancellable(cancellation)
                     .UnorderedSearchFilter(query, m => MemberOption.Haystack(m, direction, expectingBoolean))
                     .Where(m => !m.isPseudoInherited || !codebase.types.Contains(m.declaringType))
                     .OrderBy(m => BoltCore.Configuration.groupInheritedMembers && m.isPseudoInherited)
                     .ThenByDescending(m => SearchUtility.Relevance(query, MemberOption.Haystack(m, direction, expectingBoolean))))
            {
                yield return member;
            }

            foreach (var type in codebase.types
                     .Cancellable(cancellation)
                     .Where(t => !t.IsEnum)
                     .OrderedSearchFilter(query, TypeOption.Haystack))
            {
                yield return type;
            }
        }

        public override string SearchResultLabel(object item, string query)
        {
            if (item is Type)
            {
                return TypeOption.SearchResultLabel((Type)item, query);
            }
            else if (item is Member)
            {
                return MemberOption.SearchResultLabel((Member)item, query, direction, expectingBoolean);
            }

            throw new NotSupportedException();
        }

        #endregion

        #region Favorites

        public override ICollection<object> favorites { get; }

        public override string FavoritesLabel(object item)
        {
            if (item is Namespace)
            {
                return ((Namespace)item).DisplayName();
            }
            else if (item is Type)
            {
                return ((Type)item).DisplayName();
            }
            else if (item is Member)
            {
                var member = (Member)item;

                if (member.isInvocable)
                {
                    return $"{member.info.DisplayName(direction, expectingBoolean)} ({member.methodBase.DisplayParameterString(member.targetType)})";
                }
                else
                {
                    return member.info.DisplayName(direction, expectingBoolean);
                }
            }

            throw new NotSupportedException();
        }

        public override bool CanFavorite(object item)
        {
            return item is Member;
        }

        public override void OnFavoritesChange()
        {
            BoltCore.Configuration.Save();
        }

        private class Favorites : ICollection<object>
        {
            public Favorites(MemberOptionTree tree)
            {
                this.tree = tree;
            }

            private readonly MemberOptionTree tree;

            private IEnumerable<Member> validFavorites => BoltCore.Configuration.favoriteMembers.Where(tree.codebase.ValidateMember);

            public int Count => validFavorites.Count();

            public bool IsReadOnly => false;

            public IEnumerator<object> GetEnumerator()
            {
                foreach (var favorite in validFavorites)
                {
                    yield return favorite;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Contains(object item)
            {
                return validFavorites.Contains((Member)item);
            }

            public void Add(object item)
            {
                favorites.Add(item);
            }

            public bool Remove(object item)
            {
                return favorites.Remove(item);
            }

            public void Clear()
            {
                favorites.Clear();
            }

            public void CopyTo(object[] array, int arrayIndex)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                }

                if (array.Length - arrayIndex < Count)
                {
                    throw new ArgumentException();
                }

                var i = 0;

                foreach (var item in this)
                {
                    array[i + arrayIndex] = item;
                    i++;
                }
            }

            private static readonly ICollection<object> favorites = new VariantCollection<object, Member>(BoltCore.Configuration.favoriteMembers);
        }

        #endregion
    }
}
