namespace Unity.VisualScripting
{
    public class FuzzyOptionProvider : SingleDecoratorProvider<object, IFuzzyOption, FuzzyOptionAttribute>
    {
        static FuzzyOptionProvider()
        {
            instance = new FuzzyOptionProvider();
        }

        public static FuzzyOptionProvider instance { get; private set; }

        protected override bool cache => false;

        public override bool IsValid(object decorated)
        {
            return true;
        }
    }
}
