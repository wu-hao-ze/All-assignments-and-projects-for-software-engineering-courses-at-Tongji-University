using System.Globalization;
using System.Text;

namespace Unity.VisualScripting.Dependencies.NCalc
{
    public class SerializationVisitor : LogicalExpressionVisitor
    {
        public SerializationVisitor()
        {
            Result = new StringBuilder();
            _numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
        }

        private readonly NumberFormatInfo _numberFormatInfo;

        public StringBuilder Result { get; protected set; }

        public override void Visit(TernaryExpression ternary)
        {
            EncapsulateNoValue(ternary.LeftExpression);

            Result.Append("? ");

            EncapsulateNoValue(ternary.MiddleExpression);

            Result.Append(": ");

            EncapsulateNoValue(ternary.RightExpression);
        }

        public override void Visit(BinaryExpression binary)
        {
            EncapsulateNoValue(binary.LeftExpression);

            switch (binary.Type)
            {
                case BinaryExpressionType.And:
                    Result.Append("and ");
                    break;

                case BinaryExpressionType.Or:
                    Result.Append("or ");
                    break;

                case BinaryExpressionType.Div:
                    Result.Append("/ ");
                    break;

                case BinaryExpressionType.Equal:
                    Result.Append("= ");
                    break;

                case BinaryExpressionType.Greater:
                    Result.Append("> ");
                    break;

                case BinaryExpressionType.GreaterOrEqual:
                    Result.Append(">= ");
                    break;

                case BinaryExpressionType.Lesser:
                    Result.Append("< ");
                    break;

                case BinaryExpressionType.LesserOrEqual:
                    Result.Append("<= ");
                    break;

                case BinaryExpressionType.Minus:
                    Result.Append("- ");
                    break;

                case BinaryExpressionType.Modulo:
                    Result.Append("% ");
                    break;

                case BinaryExpressionType.NotEqual:
                    Result.Append("!= ");
                    break;

                case BinaryExpressionType.Plus:
                    Result.Append("+ ");
                    break;

                case BinaryExpressionType.Times:
                    Result.Append("* ");
                    break;

                case BinaryExpressionType.BitwiseAnd:
                    Result.Append("& ");
                    break;

                case BinaryExpressionType.BitwiseOr:
                    Result.Append("| ");
                    break;

                case BinaryExpressionType.BitwiseXOr:
                    Result.Append("~ ");
                    break;

                case BinaryExpressionType.LeftShift:
                    Result.Append("<< ");
                    break;

                case BinaryExpressionType.RightShift:
                    Result.Append(">> ");
                    break;
            }

            EncapsulateNoValue(binary.RightExpression);
        }

        public override void Visit(UnaryExpression unary)
        {
            switch (unary.Type)
            {
                case UnaryExpressionType.Not:
                    Result.Append("!");
                    break;

                case UnaryExpressionType.Negate:
                    Result.Append("-");
                    break;

                case UnaryExpressionType.BitwiseNot:
                    Result.Append("~");
                    break;
            }

            EncapsulateNoValue(unary.Expression);
        }

        public override void Visit(ValueExpression value)
        {
            switch (value.Type)
            {
                case ValueType.Boolean:
                    Result.Append(value.Value).Append(" ");
                    break;

                case ValueType.DateTime:
                    Result.Append("#").Append(value.Value).Append("#").Append(" ");
                    break;

                case ValueType.Float:
                    Result.Append(decimal.Parse(value.Value.ToString()).ToString(_numberFormatInfo)).Append(" ");
                    break;

                case ValueType.Integer:
                    Result.Append(value.Value).Append(" ");
                    break;

                case ValueType.String:
                    Result.Append("'").Append(value.Value).Append("'").Append(" ");
                    break;
            }
        }

        public override void Visit(FunctionExpression function)
        {
            Result.Append(function.Identifier.Name);

            Result.Append("(");

            for (var i = 0; i < function.Expressions.Length; i++)
            {
                function.Expressions[i].Accept(this);

                if (i < function.Expressions.Length - 1)
                {
                    Result.Remove(Result.Length - 1, 1);
                    Result.Append(", ");
                }
            }

            // Trim spaces before adding a closing parenthesis
            while (Result[Result.Length - 1] == ' ')
            {
                Result.Remove(Result.Length - 1, 1);
            }

            Result.Append(") ");
        }

        public override void Visit(IdentifierExpression identifier)
        {
            Result.Append("[").Append(identifier.Name).Append("] ");
        }

        protected void EncapsulateNoValue(LogicalExpression expression)
        {
            if (expression is ValueExpression)
            {
                expression.Accept(this);
            }
            else
            {
                Result.Append("(");
                expression.Accept(this);

                // Trim spaces before adding a closing parenthesis
                while (Result[Result.Length - 1] == ' ')
                {
                    Result.Remove(Result.Length - 1, 1);
                }

                Result.Append(") ");
            }
        }
    }
}
