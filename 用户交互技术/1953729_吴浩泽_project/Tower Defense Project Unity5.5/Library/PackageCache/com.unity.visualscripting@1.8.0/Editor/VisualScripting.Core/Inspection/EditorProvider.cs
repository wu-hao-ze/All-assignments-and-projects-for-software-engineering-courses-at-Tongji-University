using System;

namespace Unity.VisualScripting
{
    public class EditorProvider : SingleDecoratorProvider<Metadata, Inspector, EditorAttribute>
    {
        protected override bool cache => true;

        protected override Inspector CreateDecorator(Type decoratorType, Metadata metadata)
        {
            var inspector = base.CreateDecorator(decoratorType, metadata);
            inspector.Initialize();
            return inspector;
        }

        protected override Type GetDecoratedType(Metadata metadata)
        {
            return metadata.definedType;
        }

        public override bool IsValid(Metadata decorated)
        {
            return decorated.isLinked;
        }

        protected override Type ResolveDecoratorType(Type decoratedType)
        {
            return ResolveDecoratorTypeByHierarchy(decoratedType) ?? typeof(UnknownEditor);
        }

        public bool HasPanel(Type type)
        {
            return GetDecoratorType(type) != typeof(UnknownEditor);
        }

        static EditorProvider()
        {
            instance = new EditorProvider();
            EditorApplicationUtility.onSelectionChange += instance.FreeAll;
        }

        public static EditorProvider instance { get; private set; }
    }

    public static class XEditorProvider
    {
        public static Inspector Editor(this Metadata metadata)
        {
            return EditorProvider.instance.GetDecorator(metadata);
        }

        public static TInspector Editor<TInspector>(this Metadata metadata) where TInspector : Inspector
        {
            return EditorProvider.instance.GetDecorator<TInspector>(metadata);
        }

        public static bool HasEditor(this Type type)
        {
            return EditorProvider.instance.HasPanel(type);
        }

        public static bool HasEditor(this Metadata metadata)
        {
            return EditorProvider.instance.HasPanel(metadata.definedType);
        }
    }
}
