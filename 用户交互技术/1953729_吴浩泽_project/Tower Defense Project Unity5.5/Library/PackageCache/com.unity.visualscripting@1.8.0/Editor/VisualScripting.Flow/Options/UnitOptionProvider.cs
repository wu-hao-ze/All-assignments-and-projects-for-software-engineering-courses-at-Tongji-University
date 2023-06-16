namespace Unity.VisualScripting
{
    public static class XUnitOptionProvider
    {
        public static IUnitOption Option(this IUnit unit)
        {
            return FuzzyOptionProvider.instance.GetDecorator<IUnitOption>(unit);
        }

        public static IUnitOption Option<TOption>(this IUnit unit) where TOption : IUnitOption
        {
            return FuzzyOptionProvider.instance.GetDecorator<TOption>(unit);
        }
    }
}
