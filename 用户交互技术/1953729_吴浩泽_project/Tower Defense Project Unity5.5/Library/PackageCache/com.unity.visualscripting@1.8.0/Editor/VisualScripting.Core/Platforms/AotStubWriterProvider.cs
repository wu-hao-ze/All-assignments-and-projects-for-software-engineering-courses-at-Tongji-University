namespace Unity.VisualScripting
{
    public class AotStubWriterProvider : SingleDecoratorProvider<object, AotStubWriter, AotStubWriterAttribute>
    {
        static AotStubWriterProvider()
        {
            instance = new AotStubWriterProvider();
        }

        public static AotStubWriterProvider instance { get; }

        protected override bool cache => true;

        public override bool IsValid(object decorated)
        {
            return true;
        }
    }
}
