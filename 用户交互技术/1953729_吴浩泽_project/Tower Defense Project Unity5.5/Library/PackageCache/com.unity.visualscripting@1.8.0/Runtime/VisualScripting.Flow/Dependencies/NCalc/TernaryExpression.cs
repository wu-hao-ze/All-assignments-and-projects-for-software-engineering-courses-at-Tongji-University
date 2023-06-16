namespace Unity.VisualScripting.Dependencies.NCalc
{
    public class TernaryExpression : LogicalExpression
    {
        public TernaryExpression(LogicalExpression leftExpression, LogicalExpression middleExpression, LogicalExpression rightExpression)
        {
            LeftExpression = leftExpression;
            MiddleExpression = middleExpression;
            RightExpression = rightExpression;
        }

        public LogicalExpression LeftExpression { get; set; }

        public LogicalExpression MiddleExpression { get; set; }

        public LogicalExpression RightExpression { get; set; }

        public override void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
