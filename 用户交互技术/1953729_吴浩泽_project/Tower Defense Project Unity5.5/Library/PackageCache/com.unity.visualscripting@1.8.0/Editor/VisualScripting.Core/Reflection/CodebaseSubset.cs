using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public sealed class CodebaseSubset
    {
        private CodebaseSubset(IEnumerable<Type> types, MemberFilter memberFilter, TypeFilter memberTypeFilter = null)
        {
            Ensure.That(nameof(types)).IsNotNull(types);
            Ensure.That(nameof(memberFilter)).IsNotNull(memberFilter);

            this.types = types.ToHashSet();
            this.memberFilter = memberFilter;
            this.memberTypeFilter = memberTypeFilter;
        }

        private CodebaseSubset(IEnumerable<Type> typeSet, TypeFilter typeFilter, MemberFilter memberFilter, TypeFilter memberTypeFilter = null)
        {
            Ensure.That(nameof(typeSet)).IsNotNull(typeSet);
            Ensure.That(nameof(typeFilter)).IsNotNull(typeFilter);
            Ensure.That(nameof(memberFilter)).IsNotNull(memberFilter);

            this.typeSet = typeSet;
            this.typeFilter = typeFilter;
            this.memberFilter = memberFilter;
            this.memberTypeFilter = memberTypeFilter;
        }

        private readonly object @lock = new object();

        private bool ready;

        public IEnumerable<Type> typeSet { get; }
        public TypeFilter typeFilter { get; }
        public MemberFilter memberFilter { get; }
        public TypeFilter memberTypeFilter { get; }
        public HashSet<Type> types { get; private set; }
        public HashSet<Member> members { get; private set; }

        public void Cache()
        {
            lock (@lock)
            {
                if (ready)
                {
                    return;
                }

                ProgressUtility.DisplayProgressBar("Analyzing codebase...", null, 0);

                if (typeSet != null)
                {
                    types = typeSet.Where(typeFilter.ValidateType)
                        .ToHashSet();
                }

                members = new HashSet<Member>();

                var progress = 0f;

                foreach (var type in types)
                {
                    ProgressUtility.DisplayProgressBar("Analyzing codebase...", type.DisplayName(), progress++ / types.Count);

                    if (type.IsEnum)
                    {
                        continue;
                    }

                    members.UnionWith(FilterMembers(type));
                }

                ProgressUtility.ClearProgressBar();

                ready = true;
            }
        }

        public bool ValidateType(Type type)
        {
            return types.Contains(type);
        }

        public bool ValidateMember(Member member)
        {
            return ValidateType(member.targetType) && memberFilter.ValidateMember(member.info, memberTypeFilter);
        }

        public IEnumerable<Member> FilterMembers(Type type)
        {
            foreach (var member in type.GetMembers(memberFilter.validBindingFlags)
                     .Where(member => memberFilter.validMemberTypes.HasFlag(member.MemberType) && memberFilter.ValidateMember(member, memberTypeFilter))
                     .Select(member => member.ToManipulator(type)))
            {
                yield return member;
            }

            if (memberFilter.Methods && memberFilter.Extensions)
            {
                foreach (var member in type.GetExtensionMethods(memberFilter.Inherited)
                         .Where(method => ValidateType(method.DeclaringType) && memberFilter.ValidateMember(method, memberTypeFilter))
                         .Select(member => member.ToManipulator(type)))
                {
                    yield return member;
                }
            }
        }

        private static readonly Dictionary<Query, CodebaseSubset> cache = new Dictionary<Query, CodebaseSubset>();

        internal static CodebaseSubset Get(IEnumerable<Type> types, MemberFilter memberFilter, TypeFilter memberTypeFilter = null)
        {
            var query = new Query(types, memberFilter, memberTypeFilter);

            if (!cache.ContainsKey(query))
            {
                cache.Add(query, new CodebaseSubset(types, memberFilter, memberTypeFilter));
            }

            return cache[query];
        }

        internal static CodebaseSubset Get(IEnumerable<Type> typeSet, TypeFilter typeFilter, MemberFilter memberFilter, TypeFilter memberTypeFilter = null)
        {
            var query = new Query(typeSet, typeFilter, memberFilter, memberTypeFilter);

            if (!cache.ContainsKey(query))
            {
                cache.Add(query, new CodebaseSubset(typeSet, typeFilter, memberFilter, memberTypeFilter));
            }

            return cache[query];
        }

        private struct Query
        {
            public IEnumerable<Type> types { get; }
            public IEnumerable<Type> typeSet { get; }
            public TypeFilter typeFilter { get; }
            public MemberFilter memberFilter { get; }
            public TypeFilter memberTypeFilter { get; }

            public Query(IEnumerable<Type> typeSet, TypeFilter typeFilter, MemberFilter memberFilter, TypeFilter memberTypeFilter)
            {
                types = null;

                this.typeSet = typeSet;
                this.typeFilter = typeFilter;
                this.memberFilter = memberFilter;
                this.memberTypeFilter = memberTypeFilter;
            }

            public Query(IEnumerable<Type> types, MemberFilter memberFilter, TypeFilter memberTypeFilter)
            {
                typeSet = null;
                typeFilter = null;

                this.types = types;
                this.memberFilter = memberFilter;
                this.memberTypeFilter = memberTypeFilter;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 17;

                    hash = hash * 23 + (typeSet != null).GetHashCode();

                    if (typeSet != null)
                    {
                        foreach (var type in typeSet)
                        {
                            hash = hash * 23 + (type?.GetHashCode() ?? 0);
                        }
                    }
                    else
                    {
                        foreach (var type in types)
                        {
                            hash = hash * 23 + (type?.GetHashCode() ?? 0);
                        }
                    }

                    hash = hash * 23 + (typeFilter?.GetHashCode() ?? 0);
                    hash = hash * 23 + (memberFilter?.GetHashCode() ?? 0);
                    hash = hash * 23 + (memberTypeFilter?.GetHashCode() ?? 0);

                    return hash;
                }
            }
        }
    }
}
