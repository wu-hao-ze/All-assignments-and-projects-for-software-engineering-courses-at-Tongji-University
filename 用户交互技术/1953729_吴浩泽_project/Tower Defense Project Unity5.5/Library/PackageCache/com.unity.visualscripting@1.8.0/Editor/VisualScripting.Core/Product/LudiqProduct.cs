namespace Unity.VisualScripting
{
    [Product(ID)]
    public sealed class LudiqProduct : Product
    {
        public LudiqProduct() { }

        public override string configurationPanelLabel => "";

        public override string name => "Ludiq Framework";
        public override string description => "";
        public override string authorLabel => "";
        public override string author => "";
        public override string copyrightHolder => "Unity";
        public override string supportUrl => "";
        public override SemanticVersion version => PackageVersionUtility.version;
        public const string ID = "Ludiq";

        public const int ToolsMenuPriority = -990000;
        public const int DeveloperToolsMenuPriority = ToolsMenuPriority + 5000;

        public static LudiqProduct instance => (LudiqProduct)ProductContainer.GetProduct(ID);
    }
}
