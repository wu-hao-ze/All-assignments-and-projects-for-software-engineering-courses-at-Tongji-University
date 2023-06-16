namespace Unity.VisualScripting.Dependencies.NCalc
{
    public abstract class LogicalExpressionVisitor
    {
        public abstract void Visit(TernaryExpression ternary);
        public abstract void Visit(BinaryExpression binary);
        public abstract void Visit(UnaryExpression unary);
        public abstract void Visit(ValueExpression value);
        public abstract void Visit(FunctionExpression function);
        public abstract void Visit(IdentifierExpression identifier);
    }
}
