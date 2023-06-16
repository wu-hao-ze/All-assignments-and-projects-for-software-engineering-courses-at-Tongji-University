using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public abstract class MultiDecoratorProvider<TDecorated, TDecorator, TAttribute>
        where TAttribute : Attribute, IDecoratorAttribute
    {
        protected MultiDecoratorProvider()
        {
            definedDecoratorTypes = new Dictionary<Type, HashSet<Type>>();
            resolvedDecoratorTypes = new Dictionary<Type, HashSet<Type>>();
            decorators = new Dictionary<TDecorated, HashSet<TDecorator>>();

            foreach (var decoratorType in typeset.Where(t => t.HasAttribute<TAttribute>(false)))
            {
                foreach (var decoratedType in decoratorType.GetAttributes<TAttribute>(false).Select(a => a.type))
                {
                    if (!definedDecoratorTypes.ContainsKey(decoratedType))
                    {
                        definedDecoratorTypes.Add(decoratedType, new HashSet<Type>());
                    }

                    definedDecoratorTypes[decoratedType].Add(decoratorType);
                }
            }
        }

        protected virtual IEnumerable<Type> typeset => Codebase.editorTypes;

        private readonly Dictionary<TDecorated, HashSet<TDecorator>> decorators;
        protected readonly Dictionary<Type, HashSet<Type>> definedDecoratorTypes;
        private readonly Dictionary<Type, HashSet<Type>> resolvedDecoratorTypes;

        protected virtual IEnumerable<TDecorator> CreateDecorators(TDecorated decorated)
        {
            return GetDecoratorTypes(decorated.GetType()).Select(t => (TDecorator)t.Instantiate(false, decorated));
        }

        protected virtual bool ShouldInvalidateDecorators(TDecorated decorated)
        {
            return false;
        }

        public IEnumerable<TDecorator> GetDecorators(TDecorated decorated)
        {
            lock (decorators)
            {
                if (decorators.ContainsKey(decorated) && ShouldInvalidateDecorators(decorated))
                {
                    foreach (var disposableDecorator in decorators[decorated].OfType<IDisposable>())
                    {
                        disposableDecorator.Dispose();
                    }

                    decorators.Remove(decorated);
                }

                if (!decorators.ContainsKey(decorated))
                {
                    decorators.Add(decorated, CreateDecorators(decorated).ToHashSet());
                }

                return decorators[decorated];
            }
        }

        public IEnumerable<TSpecificDecorator> GetDecorators<TSpecificDecorator>(TDecorated decorated) where TSpecificDecorator : TDecorator
        {
            return GetDecorators(decorated).OfType<TSpecificDecorator>();
        }

        public virtual void ClearCache()
        {
            lock (decorators)
            {
                foreach (var decorator in decorators.SelectMany(kvp => kvp.Value).OfType<IDisposable>())
                {
                    decorator.Dispose();
                }

                decorators.Clear();
            }
        }

        public bool HasDecorator(Type type)
        {
            lock (decorators)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                if (!resolvedDecoratorTypes.ContainsKey(type))
                {
                    resolvedDecoratorTypes.Add(type, ResolveDecoratorTypes(type).ToHashSet());
                }

                return resolvedDecoratorTypes[type] != null;
            }
        }

        public IEnumerable<Type> GetDecoratorTypes(Type type)
        {
            lock (decorators)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                if (!HasDecorator(type))
                {
                    throw new NotSupportedException($"Cannot resolve decorator type for '{type}'.");
                }

                return resolvedDecoratorTypes[type];
            }
        }

        protected virtual IEnumerable<Type> ResolveDecoratorTypes(Type type)
        {
            return ResolveDecoratorTypesByHierarchy(type, true);
        }

        protected IEnumerable<Type> ResolveDecoratorTypesByHierarchy(Type type, bool inherit)
        {
            // We traverse the tree recursively and manually instead of
            // using Linq to find any "assignable from" type in the defined
            // decorators list in order to preserve priority.

            // For example, in an A : B : C chain where we have decorators
            // for B and C, this method will always map A to B, not A to C.

            var resolved = DirectResolve(type) ?? GenericResolve(type);

            if (resolved != null)
            {
                return resolved;
            }

            if (inherit)
            {
                foreach (var baseTypeOrInterface in type.BaseTypeAndInterfaces(false))
                {
                    var baseResolved = ResolveDecoratorTypesByHierarchy(baseTypeOrInterface, false);

                    if (baseResolved != null)
                    {
                        return baseResolved;
                    }
                }

                if (type.BaseType != null)
                {
                    var baseResolved = ResolveDecoratorTypesByHierarchy(type.BaseType, true);

                    if (baseResolved != null)
                    {
                        return baseResolved;
                    }
                }
            }

            return null;
        }

        private IEnumerable<Type> DirectResolve(Type type)
        {
            if (definedDecoratorTypes.ContainsKey(type))
            {
                return definedDecoratorTypes[type];
            }

            return null;
        }

        private IEnumerable<Type> GenericResolve(Type type)
        {
            if (type.IsGenericType)
            {
                var typeDefinition = type.GetGenericTypeDefinition();

                if (definedDecoratorTypes.ContainsKey(typeDefinition))
                {
                    foreach (var definedDecoratorType in definedDecoratorTypes[typeDefinition])
                    {
                        // For example: [Decorator(List<>)] ListDecorator<T> gets passed T of the item
                        if (definedDecoratorType.ContainsGenericParameters)
                        {
                            yield return definedDecoratorType.MakeGenericType(type.GetGenericArguments());
                        }
                        else
                        {
                            yield return definedDecoratorType;
                        }
                    }
                }
            }
        }
    }
}
