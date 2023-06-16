using System;
using System.Collections;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public class InspectorProvider : SingleDecoratorProvider<Metadata, Inspector, InspectorAttribute>
    {
        protected override bool cache => true;

        public override bool IsValid(Metadata metadata)
        {
            return metadata.isLinked;
        }

        protected override Inspector CreateDecorator(Type decoratorType, Metadata metadata)
        {
            var inspector = base.CreateDecorator(decoratorType, metadata);
            inspector.Initialize();
            return inspector;
        }

        protected override Type GetDecoratedType(Metadata metadata)
        {
            var inspectedType = metadata.definedType;

            foreach (var attribute in metadata.GetAttributes<Attribute>())
            {
                var attributeType = attribute.GetType();

                if (HasInspector(attributeType))
                {
                    inspectedType = attributeType;
                }
            }

            return inspectedType;
        }

        protected override Type ResolveDecoratorType(Type decoratedType)
        {
            return
                UnityObjectReferenceInspector(decoratedType) ??
                ResolveDecoratorTypeByHierarchy(decoratedType) ??
                ImplementationInspector(decoratedType) ??
                EnumInspector(decoratedType) ??
                NullableInspector(decoratedType) ??
                ListInspector(decoratedType) ??
                DictionaryInspector(decoratedType) ??
                SystemObjectInspector(decoratedType) ??
                CustomPropertyDrawerInspector(decoratedType) ??
                AutomaticReflectedInspector(decoratedType) ??
                TypeHandleInspector(decoratedType) ??
                typeof(UnknownInspector);
        }

        protected Type UnityObjectReferenceInspector(Type type)
        {
            if (typeof(UnityObject).IsAssignableFrom(type))
            {
                return typeof(UnityObjectInspector);
            }

            return null;
        }

        private Type EnumInspector(Type type)
        {
            if (type.IsEnum)
            {
                return typeof(EnumInspector);
            }

            return null;
        }

        private Type ListInspector(Type type)
        {
            if (type.IsArray || (typeof(IList).IsAssignableFrom(type) && type.IsConcrete()))
            {
                return typeof(ListInspector);
            }

            return null;
        }

        private Type DictionaryInspector(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type) && type.IsConcrete())
            {
                return typeof(DictionaryInspector);
            }

            return null;
        }

        private Type NullableInspector(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return typeof(NullableInspector);
            }

            return null;
        }

        private Type ImplementationInspector(Type type)
        {
            if (type.HasAttribute<InspectViaImplementationsAttribute>())
            {
                return typeof(ImplementationInspector<>).MakeGenericType(type);
                ;
            }

            // For the time being, we'll restrict this inspector to the [InspectViaImplementations] attribute.
            // Otherwise, IList, IEnumerable, IComparable, etc. all get an implementation inspector,
            // which is silly. There should however be a way of registering this without having
            // access to the type. Or maybe there's a clever filter to apply here instead.
            /*if (!type.IsConcrete() && definedDecoratorTypes.Keys.Any(type.IsAssignableFrom))
            {
                return typeof(ImplementationInspector<>).MakeGenericType(type);
            }*/

            return null;
        }

        private Type SystemObjectInspector(Type type)
        {
            if (type == typeof(object))
            {
                return typeof(SystemObjectInspector);
            }

            return null;
        }

        private Type TypeHandleInspector(Type type)
        {
            return type == typeof(SerializableType) ? typeof(TypeHandleInspector) : null;
        }

        private Type AutomaticReflectedInspector(Type type)
        {
            if (type.HasAttribute<InspectableAttribute>())
            {
                return typeof(AutomaticReflectedInspector);
            }

            return null;
        }

        private Type CustomPropertyDrawerInspector(Type type)
        {
            if (SerializedPropertyProviderProvider.instance.HasDecorator(type))
            {
                return typeof(CustomPropertyDrawerInspector);
            }

            return null;
        }

        public bool HasInspector(Type type)
        {
            return GetDecoratorType(type) != typeof(UnknownInspector);
        }

        static InspectorProvider()
        {
            instance = new InspectorProvider();
            EditorApplicationUtility.onSelectionChange += instance.FreeAll;
        }

        public static InspectorProvider instance { get; private set; }
    }

    public static class XInspectorProvider
    {
        public static Inspector Inspector(this Metadata metadata)
        {
            return InspectorProvider.instance.GetDecorator(metadata);
        }

        public static TInspector Inspector<TInspector>(this Metadata metadata) where TInspector : Inspector
        {
            return InspectorProvider.instance.GetDecorator<TInspector>(metadata);
        }

        public static bool HasInspector(this Type type)
        {
            return InspectorProvider.instance.HasInspector(type);
        }

        public static bool HasInspector(this Metadata metadata)
        {
            return InspectorProvider.instance.HasInspector(metadata.definedType);
        }
    }
}
