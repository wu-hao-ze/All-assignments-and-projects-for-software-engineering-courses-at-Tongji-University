using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class SingleDecoratorProvider<TDecorated, TDecorator, TAttribute>
        where TAttribute : Attribute, IDecoratorAttribute
    {
        protected readonly object typesLock = new object();
        protected readonly object instancesLock = new object();

        protected SingleDecoratorProvider()
        {
            definedDecoratorTypes = new Dictionary<Type, Type>();
            resolvedDecoratorTypes = new Dictionary<Type, Type>();

            decorators = new Dictionary<TDecorated, TDecorator>();
            decorateds = new Dictionary<TDecorator, TDecorated>();

            MapAttributeTypes();

            Freed();

            EditorApplication.update += FreeIfNeeded;
        }

        protected virtual TDecorator CreateDecorator(Type decoratorType, TDecorated decorated)
        {
            return (TDecorator)decoratorType.Instantiate(false, decorated);
        }

        private TDecorator CreateDecorator(TDecorated decorated)
        {
            if (!IsValid(decorated))
            {
                throw new InvalidOperationException($"Decorated object is not valid: {decorated}");
            }

            return CreateDecorator(GetDecoratorType(GetDecoratedType(decorated)), decorated);
        }

        #region Type Resolution

        // By restricting our search in editor types, we greatly reduce the enumeration
        // and therefore the attribute caching time on initialization
        protected virtual IEnumerable<Type> typeset => Codebase.editorTypes;

        protected readonly Dictionary<Type, Type> definedDecoratorTypes;

        protected readonly Dictionary<Type, Type> resolvedDecoratorTypes;

        private void MapAttributeTypes()
        {
            foreach (var decoratorType in typeset.Where(t => t.HasAttribute<TAttribute>(false)))
            {
                foreach (var decoratedType in decoratorType.GetAttributes<TAttribute>(false).Select(a => a.type))
                {
                    if (definedDecoratorTypes.ContainsKey(decoratedType))
                    {
                        Debug.LogWarning($"Multiple '{typeof(TDecorator)}' for '{decoratedType}'. Ignoring '{decoratorType}'.");
                        continue;
                    }

                    definedDecoratorTypes.Add(decoratedType, decoratorType);
                }
            }
        }

        public bool HasDecorator(Type decoratedType)
        {
            return TryGetDecoratorType(decoratedType, out var decoratorType);
        }

        public bool TryGetDecoratorType(Type decoratedType, out Type decoratorType)
        {
            lock (typesLock)
            {
                Ensure.That(nameof(decoratedType)).IsNotNull(decoratedType);

                if (!resolvedDecoratorTypes.TryGetValue(decoratedType, out decoratorType))
                {
                    decoratorType = ResolveDecoratorType(decoratedType);
                    resolvedDecoratorTypes.Add(decoratedType, decoratorType);
                }

                return decoratorType != null;
            }
        }

        protected virtual Type GetDecoratedType(TDecorated decorated)
        {
            return decorated.GetType();
        }

        public Type GetDecoratorType(Type decoratedType)
        {
            lock (typesLock)
            {
                Ensure.That(nameof(decoratedType)).IsNotNull(decoratedType);

                if (!TryGetDecoratorType(decoratedType, out var decoratorType))
                {
                    throw new NotSupportedException($"Cannot decorate '{decoratedType}' with '{typeof(TDecorator)}'.");
                }

                return decoratorType;
            }
        }

        protected virtual Type ResolveDecoratorType(Type decoratedType)
        {
            return ResolveDecoratorTypeByHierarchy(decoratedType);
        }

        protected Type ResolveDecoratorTypeByHierarchy(Type decoratedType, bool inherit = true)
        {
            // We traverse the tree recursively and manually instead of
            // using Linq to find any "assignable from" type in the defined
            // decorators list in order to preserve priority.

            // For example, in an A : B : C chain where we have decorators
            // for B and C, this method will always map A to B, not A to C.

            var resolved = DirectResolve(decoratedType) ?? GenericResolve(decoratedType);

            if (resolved != null)
            {
                return resolved;
            }

            if (inherit)
            {
                foreach (var baseTypeOrInterface in decoratedType.BaseTypeAndInterfaces(false))
                {
                    var baseResolved = ResolveDecoratorTypeByHierarchy(baseTypeOrInterface, false);

                    if (baseResolved != null)
                    {
                        return baseResolved;
                    }
                }

                if (decoratedType.BaseType != null)
                {
                    var baseResolved = ResolveDecoratorTypeByHierarchy(decoratedType.BaseType);

                    if (baseResolved != null)
                    {
                        return baseResolved;
                    }
                }
            }

            return null;
        }

        private Type DirectResolve(Type decoratedType)
        {
            if (definedDecoratorTypes.ContainsKey(decoratedType))
            {
                var definedDecoratorType = definedDecoratorTypes[decoratedType];

                if (definedDecoratorType.IsGenericTypeDefinition)
                {
                    var arguments = definedDecoratorType.GetGenericArguments();

                    // For example: [Decorator(Decorated)] Decorator<TDecorated> gets properly closed-constructed with type
                    if (arguments.Length == 1 && arguments[0].CanMakeGenericTypeVia(decoratedType))
                    {
                        return definedDecoratorType.MakeGenericType(decoratedType);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return definedDecoratorTypes[decoratedType];
                }
            }

            return null;
        }

        private Type GenericResolve(Type decoratedType)
        {
            if (decoratedType.IsGenericType)
            {
                var typeDefinition = decoratedType.GetGenericTypeDefinition();

                if (definedDecoratorTypes.ContainsKey(typeDefinition))
                {
                    var definedDecoratorType = definedDecoratorTypes[typeDefinition];

                    // For example: [Decorator(List<>)] ListDecorator<T> gets passed T of the item
                    if (definedDecoratorType.ContainsGenericParameters)
                    {
                        return definedDecoratorType.MakeGenericType(decoratedType.GetGenericArguments());
                    }
                    else
                    {
                        return definedDecoratorType;
                    }
                }
            }

            return null;
        }

        #endregion


        #region Cache

        protected readonly Dictionary<TDecorated, TDecorator> decorators;

        protected readonly Dictionary<TDecorator, TDecorated> decorateds;

        protected abstract bool cache { get; }

        public abstract bool IsValid(TDecorated decorated);

        public TDecorator GetDecorator(TDecorated decorated)
        {
            Ensure.That(nameof(decorated)).IsNotNull(decorated);

            if (!cache)
            {
                var decorator = CreateDecorator(decorated);
                (decorator as IInitializable)?.Initialize();
                return decorator;
            }

            lock (instancesLock)
            {
                var decoratorExists = decorators.TryGetValue(decorated, out var decorator);

                if (decoratorExists && !IsValid(decorateds[decorator]))
                {
                    Free(decorator);

                    decoratorExists = false;
                }

                if (!decoratorExists)
                {
                    decorator = CreateDecorator(decorated);

                    decorators.Add(decorated, decorator);
                    decorateds.Add(decorator, decorated);

                    (decorator as IInitializable)?.Initialize();
                }

                return decorator;
            }
        }

        public T GetDecorator<T>(TDecorated decorated) where T : TDecorator
        {
            var decorator = GetDecorator(decorated);

            if (!(decorator is T))
            {
                throw new InvalidCastException($"Decorator type mismatch for '{decorated}': found {decorator.GetType()}, expected {typeof(T)}.");
            }

            return (T)decorator;
        }

        #endregion


        #region Collection

        private DateTime lastFreeTime;

        protected virtual TimeSpan freeInterval => TimeSpan.FromSeconds(5);

        private void Freed()
        {
            lastFreeTime = DateTime.Now;
        }

        private bool shouldFree => cache && DateTime.Now > lastFreeTime + freeInterval;

        private void FreeIfNeeded()
        {
            if (shouldFree)
            {
                FreeInvalid();
            }
        }

        public void Free(TDecorator decorator)
        {
            lock (instancesLock)
            {
                if (decorateds.ContainsKey(decorator))
                {
                    (decorator as IDisposable)?.Dispose();
                    var decorated = decorateds[decorator];
                    decorateds.Remove(decorator);
                    decorators.Remove(decorated);
                }
            }
        }

        public void Free(IEnumerable<TDecorator> decorators)
        {
            foreach (var decorator in decorators)
            {
                Free(decorator);
            }
        }

        public void FreeInvalid()
        {
            if (!cache)
            {
                Debug.LogWarning($"Trying to free a decorator provider without caching: {this}");

                return;
            }

            lock (instancesLock)
            {
                Free(decorators.Where(d => !IsValid(d.Key)).Select(d => d.Value).ToArray());
                Freed();
            }
        }

        public void FreeAll()
        {
            if (!cache)
            {
                Debug.LogWarning($"Trying to free a decorator provider without caching: {this}");

                return;
            }

            lock (instancesLock)
            {
                Free(decorators.Values.ToArray());
                Freed();
            }
        }

        #endregion

        public void Renew<TSpecificDecorator>(ref TSpecificDecorator decorator, TDecorated decorated, Func<TDecorated, TSpecificDecorator> constructor = null) where TSpecificDecorator : TDecorator
        {
            if (decorator == null || !IsValid(decorated))
            {
                if (constructor != null)
                {
                    decorator = constructor(decorated);
                }
                else
                {
                    decorator = (TSpecificDecorator)CreateDecorator(typeof(TSpecificDecorator), decorated);
                }
            }
        }
    }
}
