using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public static class OperatorUtility
    {
        static OperatorUtility()
        {
            unaryOperatorHandlers.Add(UnaryOperator.LogicalNegation, logicalNegationHandler);
            unaryOperatorHandlers.Add(UnaryOperator.NumericNegation, numericNegationHandler);
            unaryOperatorHandlers.Add(UnaryOperator.Increment, incrementHandler);
            unaryOperatorHandlers.Add(UnaryOperator.Decrement, decrementHandler);
            unaryOperatorHandlers.Add(UnaryOperator.Plus, plusHandler);

            binaryOpeatorHandlers.Add(BinaryOperator.Addition, additionHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.Subtraction, subtractionHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.Multiplication, multiplicationHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.Division, divisionHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.Modulo, moduloHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.And, andHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.Or, orHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.ExclusiveOr, exclusiveOrHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.Equality, equalityHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.Inequality, inequalityHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.GreaterThan, greaterThanHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.LessThan, lessThanHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.GreaterThanOrEqual, greaterThanOrEqualHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.LessThanOrEqual, lessThanOrEqualHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.LeftShift, leftShiftHandler);
            binaryOpeatorHandlers.Add(BinaryOperator.RightShift, rightShiftHandler);
        }

        // https://msdn.microsoft.com/en-us/library/2sk3x8a7(v=vs.71).aspx
        public static readonly Dictionary<string, string> operatorNames = new Dictionary<string, string>
        {
            { "op_Addition", "+" },
            { "op_Subtraction", "-" },
            { "op_Multiply", "*" },
            { "op_Division", "/" },
            { "op_Modulus", "%" },
            { "op_ExclusiveOr", "^" },
            { "op_BitwiseAnd", "&" },
            { "op_BitwiseOr", "|" },
            { "op_LogicalAnd", "&&" },
            { "op_LogicalOr", "||" },
            { "op_Assign", "=" },
            { "op_LeftShift", "<<" },
            { "op_RightShift", ">>" },
            { "op_Equality", "==" },
            { "op_GreaterThan", ">" },
            { "op_LessThan", "<" },
            { "op_Inequality", "!=" },
            { "op_GreaterThanOrEqual", ">=" },
            { "op_LessThanOrEqual", "<=" },
            { "op_MultiplicationAssignment", "*=" },
            { "op_SubtractionAssignment", "-=" },
            { "op_ExclusiveOrAssignment", "^=" },
            { "op_LeftShiftAssignment", "<<=" },
            { "op_ModulusAssignment", "%=" },
            { "op_AdditionAssignment", "+=" },
            { "op_BitwiseAndAssignment", "&=" },
            { "op_BitwiseOrAssignment", "|=" },
            { "op_Comma", "," },
            { "op_DivisionAssignment", "/=" },
            { "op_Decrement", "--" },
            { "op_Increment", "++" },
            { "op_UnaryNegation", "-" },
            { "op_UnaryPlus", "+" },
            { "op_OnesComplement", "~" },
        };

        public static readonly Dictionary<string, int> operatorRanks = new Dictionary<string, int>
        {
            { "op_Addition", 2 },
            { "op_Subtraction", 2 },
            { "op_Multiply", 2 },
            { "op_Division", 2 },
            { "op_Modulus", 2 },
            { "op_ExclusiveOr", 2 },
            { "op_BitwiseAnd", 2 },
            { "op_BitwiseOr", 2 },
            { "op_LogicalAnd", 2 },
            { "op_LogicalOr", 2 },
            { "op_Assign", 2 },
            { "op_LeftShift", 2 },
            { "op_RightShift", 2 },
            { "op_Equality", 2 },
            { "op_GreaterThan", 2 },
            { "op_LessThan", 2 },
            { "op_Inequality", 2 },
            { "op_GreaterThanOrEqual", 2 },
            { "op_LessThanOrEqual", 2 },
            { "op_MultiplicationAssignment", 2 },
            { "op_SubtractionAssignment", 2 },
            { "op_ExclusiveOrAssignment", 2 },
            { "op_LeftShiftAssignment", 2 },
            { "op_ModulusAssignment", 2 },
            { "op_AdditionAssignment", 2 },
            { "op_BitwiseAndAssignment", 2 },
            { "op_BitwiseOrAssignment", 2 },
            { "op_Comma", 2 },
            { "op_DivisionAssignment", 2 },
            { "op_Decrement", 1 },
            { "op_Increment", 1 },
            { "op_UnaryNegation", 1 },
            { "op_UnaryPlus", 1 },
            { "op_OnesComplement", 1 },
        };

        private static readonly Dictionary<UnaryOperator, UnaryOperatorHandler> unaryOperatorHandlers = new Dictionary<UnaryOperator, UnaryOperatorHandler>();
        private static readonly Dictionary<BinaryOperator, BinaryOperatorHandler> binaryOpeatorHandlers = new Dictionary<BinaryOperator, BinaryOperatorHandler>();

        private static readonly LogicalNegationHandler logicalNegationHandler = new LogicalNegationHandler();
        private static readonly NumericNegationHandler numericNegationHandler = new NumericNegationHandler();
        private static readonly IncrementHandler incrementHandler = new IncrementHandler();
        private static readonly DecrementHandler decrementHandler = new DecrementHandler();
        private static readonly PlusHandler plusHandler = new PlusHandler();

        private static readonly AdditionHandler additionHandler = new AdditionHandler();
        private static readonly SubtractionHandler subtractionHandler = new SubtractionHandler();
        private static readonly MultiplicationHandler multiplicationHandler = new MultiplicationHandler();
        private static readonly DivisionHandler divisionHandler = new DivisionHandler();
        private static readonly ModuloHandler moduloHandler = new ModuloHandler();
        private static readonly AndHandler andHandler = new AndHandler();
        private static readonly OrHandler orHandler = new OrHandler();
        private static readonly ExclusiveOrHandler exclusiveOrHandler = new ExclusiveOrHandler();
        private static readonly EqualityHandler equalityHandler = new EqualityHandler();
        private static readonly InequalityHandler inequalityHandler = new InequalityHandler();
        private static readonly GreaterThanHandler greaterThanHandler = new GreaterThanHandler();
        private static readonly LessThanHandler lessThanHandler = new LessThanHandler();
        private static readonly GreaterThanOrEqualHandler greaterThanOrEqualHandler = new GreaterThanOrEqualHandler();
        private static readonly LessThanOrEqualHandler lessThanOrEqualHandler = new LessThanOrEqualHandler();
        private static readonly LeftShiftHandler leftShiftHandler = new LeftShiftHandler();
        private static readonly RightShiftHandler rightShiftHandler = new RightShiftHandler();

        public static UnaryOperatorHandler GetHandler(UnaryOperator @operator)
        {
            if (unaryOperatorHandlers.ContainsKey(@operator))
            {
                return unaryOperatorHandlers[@operator];
            }

            throw new UnexpectedEnumValueException<UnaryOperator>(@operator);
        }

        public static BinaryOperatorHandler GetHandler(BinaryOperator @operator)
        {
            if (binaryOpeatorHandlers.ContainsKey(@operator))
            {
                return binaryOpeatorHandlers[@operator];
            }

            throw new UnexpectedEnumValueException<BinaryOperator>(@operator);
        }

        public static string Symbol(this UnaryOperator @operator)
        {
            return GetHandler(@operator).symbol;
        }

        public static string Symbol(this BinaryOperator @operator)
        {
            return GetHandler(@operator).symbol;
        }

        public static string Name(this UnaryOperator @operator)
        {
            return GetHandler(@operator).name;
        }

        public static string Name(this BinaryOperator @operator)
        {
            return GetHandler(@operator).name;
        }

        public static string Verb(this UnaryOperator @operator)
        {
            return GetHandler(@operator).verb;
        }

        public static string Verb(this BinaryOperator @operator)
        {
            return GetHandler(@operator).verb;
        }

        public static object Operate(UnaryOperator @operator, object x)
        {
            if (!unaryOperatorHandlers.ContainsKey(@operator))
            {
                throw new UnexpectedEnumValueException<UnaryOperator>(@operator);
            }

            return unaryOperatorHandlers[@operator].Operate(x);
        }

        public static object Operate(BinaryOperator @operator, object a, object b)
        {
            if (!binaryOpeatorHandlers.ContainsKey(@operator))
            {
                throw new UnexpectedEnumValueException<BinaryOperator>(@operator);
            }

            return binaryOpeatorHandlers[@operator].Operate(a, b);
        }

        public static object Negate(object x)
        {
            return numericNegationHandler.Operate(x);
        }

        public static object Not(object x)
        {
            return logicalNegationHandler.Operate(x);
        }

        public static object UnaryPlus(object x)
        {
            return plusHandler.Operate(x);
        }

        public static object Increment(object x)
        {
            return incrementHandler.Operate(x);
        }

        public static object Decrement(object x)
        {
            return decrementHandler.Operate(x);
        }

        public static object And(object a, object b)
        {
            return andHandler.Operate(a, b);
        }

        public static object Or(object a, object b)
        {
            return orHandler.Operate(a, b);
        }

        public static object ExclusiveOr(object a, object b)
        {
            return exclusiveOrHandler.Operate(a, b);
        }

        public static object Add(object a, object b)
        {
            return additionHandler.Operate(a, b);
        }

        public static object Subtract(object a, object b)
        {
            return subtractionHandler.Operate(a, b);
        }

        public static object Multiply(object a, object b)
        {
            return multiplicationHandler.Operate(a, b);
        }

        public static object Divide(object a, object b)
        {
            return divisionHandler.Operate(a, b);
        }

        public static object Modulo(object a, object b)
        {
            return moduloHandler.Operate(a, b);
        }

        public static bool Equal(object a, object b)
        {
            return (bool)equalityHandler.Operate(a, b);
        }

        public static bool NotEqual(object a, object b)
        {
            return (bool)inequalityHandler.Operate(a, b);
        }

        public static bool GreaterThan(object a, object b)
        {
            return (bool)greaterThanHandler.Operate(a, b);
        }

        public static bool LessThan(object a, object b)
        {
            return (bool)lessThanHandler.Operate(a, b);
        }

        public static bool GreaterThanOrEqual(object a, object b)
        {
            return (bool)greaterThanOrEqualHandler.Operate(a, b);
        }

        public static bool LessThanOrEqual(object a, object b)
        {
            return (bool)lessThanOrEqualHandler.Operate(a, b);
        }

        public static object LeftShift(object a, object b)
        {
            return leftShiftHandler.Operate(a, b);
        }

        public static object RightShift(object a, object b)
        {
            return rightShiftHandler.Operate(a, b);
        }
    }
}
