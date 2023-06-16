namespace Unity.VisualScripting.Dependencies.NCalc
{
    public class FunctionExpression : LogicalExpression
    {
        public FunctionExpression(IdentifierExpression identifier, LogicalExpression[] expressions)
        {
            Identifier = identifier;
            Expressions = expressions;
        }

        public IdentifierExpression Identifier { get; set; }

        public LogicalExpression[] Expressions { get; set; }

        public override void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
