namespace Unity.VisualScripting.Dependencies.NCalc
{
    public class IdentifierExpression : LogicalExpression
    {
        public IdentifierExpression(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
