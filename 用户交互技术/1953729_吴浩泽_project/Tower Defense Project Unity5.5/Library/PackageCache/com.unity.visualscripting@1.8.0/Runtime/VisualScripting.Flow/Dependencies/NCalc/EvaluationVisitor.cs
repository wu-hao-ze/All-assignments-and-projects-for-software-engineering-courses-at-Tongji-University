using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Unity.VisualScripting.Dependencies.NCalc
{
    public class EvaluationVisitor : LogicalExpressionVisitor
    {
        public EvaluationVisitor(Flow flow, EvaluateOptions options)
        {
            this.flow = flow;
            this.options = options;
        }

        public event EvaluateFunctionHandler EvaluateFunction;

        public event EvaluateParameterHandler EvaluateParameter;

        private readonly Flow flow;

        private readonly EvaluateOptions options;

        private bool IgnoreCase => options.HasFlag(EvaluateOptions.IgnoreCase);

        public object Result { get; private set; }

        public Dictionary<string, object> Parameters { get; set; }

        private object Evaluate(LogicalExpression expression)
        {
            expression.Accept(this);
            return Result;
        }

        public override void Visit(TernaryExpression ternary)
        {
            // Evaluates the left expression and saves the value
            ternary.LeftExpression.Accept(this);

            var left = ConversionUtility.Convert<bool>(Result);

            if (left)
            {
                ternary.MiddleExpression.Accept(this);
            }
            else
            {
                ternary.RightExpression.Accept(this);
            }
        }

        public override void Visit(BinaryExpression binary)
        {
            // Simulate Lazy<Func<>> behavior for late evaluation
            object leftValue = null;
            Func<object> left = () =>
            {
                if (leftValue == null)
                {
                    binary.LeftExpression.Accept(this);
                    leftValue = Result;
                }
                return leftValue;
            };

            // Simulate Lazy<Func<>> behavior for late evaluation
            object rightValue = null;
            Func<object> right = () =>
            {
                if (rightValue == null)
                {
                    binary.RightExpression.Accept(this);
                    rightValue = Result;
                }
                return rightValue;
            };

            switch (binary.Type)
            {
                case BinaryExpressionType.And:
                    Result = ConversionUtility.Convert<bool>(left()) && ConversionUtility.Convert<bool>(right());
                    break;

                case BinaryExpressionType.Or:
                    Result = ConversionUtility.Convert<bool>(left()) || ConversionUtility.Convert<bool>(right());
                    break;

                case BinaryExpressionType.Div:
                    Result = OperatorUtility.Divide(left(), right());
                    break;

                case BinaryExpressionType.Equal:
                    Result = OperatorUtility.Equal(left(), right());
                    break;

                case BinaryExpressionType.Greater:
                    Result = OperatorUtility.GreaterThan(left(), right());
                    break;

                case BinaryExpressionType.GreaterOrEqual:
                    Result = OperatorUtility.GreaterThanOrEqual(left(), right());
                    break;

                case BinaryExpressionType.Lesser:
                    Result = OperatorUtility.LessThan(left(), right());
                    break;

                case BinaryExpressionType.LesserOrEqual:
                    Result = OperatorUtility.LessThanOrEqual(left(), right());
                    break;

                case BinaryExpressionType.Minus:
                    Result = OperatorUtility.Subtract(left(), right());
                    break;

                case BinaryExpressionType.Modulo:
                    Result = OperatorUtility.Modulo(left(), right());
                    break;

                case BinaryExpressionType.NotEqual:
                    Result = OperatorUtility.NotEqual(left(), right());
                    break;

                case BinaryExpressionType.Plus:
                    Result = OperatorUtility.Add(left(), right());
                    break;

                case BinaryExpressionType.Times:
                    Result = OperatorUtility.Multiply(left(), right());
                    break;

                case BinaryExpressionType.BitwiseAnd:
                    Result = OperatorUtility.And(left(), right());
                    break;

                case BinaryExpressionType.BitwiseOr:
                    Result = OperatorUtility.Or(left(), right());
                    break;

                case BinaryExpressionType.BitwiseXOr:
                    Result = OperatorUtility.ExclusiveOr(left(), right());
                    break;

                case BinaryExpressionType.LeftShift:
                    Result = OperatorUtility.LeftShift(left(), right());
                    break;

                case BinaryExpressionType.RightShift:
                    Result = OperatorUtility.RightShift(left(), right());
                    break;
            }
        }

        public override void Visit(UnaryExpression unary)
        {
            // Recursively evaluates the underlying expression
            unary.Expression.Accept(this);

            switch (unary.Type)
            {
                case UnaryExpressionType.Not:
                    Result = !ConversionUtility.Convert<bool>(Result);
                    break;

                case UnaryExpressionType.Negate:
                    Result = OperatorUtility.Negate(Result);
                    break;

                case UnaryExpressionType.BitwiseNot:
                    Result = OperatorUtility.Not(Result);
                    break;
            }
        }

        public override void Visit(ValueExpression value)
        {
            Result = value.Value;
        }

        public override void Visit(FunctionExpression function)
        {
            var args = new FunctionArgs
            {
                Parameters = new Expression[function.Expressions.Length]
            };

            // Don't call parameters right now, instead let the function do it as needed.
            // Some parameters shouldn't be called, for instance, in a if(), the "not" value might be a division by zero
            // Evaluating every value could produce unexpected behaviour
            for (var i = 0; i < function.Expressions.Length; i++)
            {
                args.Parameters[i] = new Expression(function.Expressions[i], options);
                args.Parameters[i].EvaluateFunction += EvaluateFunction;
                args.Parameters[i].EvaluateParameter += EvaluateParameter;

                // Assign the parameters of the Expression to the arguments so that custom Functions and Parameters can use them
                args.Parameters[i].Parameters = Parameters;
            }

            // Calls external implementation
            OnEvaluateFunction(IgnoreCase ? function.Identifier.Name.ToLower() : function.Identifier.Name, args);

            // If an external implementation was found get the result back
            if (args.HasResult)
            {
                Result = args.Result;
                return;
            }

            switch (function.Identifier.Name.ToLower(CultureInfo.InvariantCulture))
            {
                case "abs":
                    CheckCase(function, "Abs");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Abs(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "acos":
                    CheckCase(function, "Acos");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Acos(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "asin":
                    CheckCase(function, "Asin");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Asin(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "atan":
                    CheckCase(function, "Atan");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Atan(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "ceil":
                    CheckCase(function, "Ceil");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Ceil(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "cos":
                    CheckCase(function, "Cos");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Cos(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "exp":
                    CheckCase(function, "Exp");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Exp(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "floor":
                    CheckCase(function, "Floor");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Floor(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "log":
                    CheckCase(function, "Log");
                    CheckExactArgumentCount(function, 2);
                    Result = Mathf.Log(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])), ConversionUtility.Convert<float>(Evaluate(function.Expressions[1])));
                    break;

                case "log10":
                    CheckCase(function, "Log10");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Log10(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "pow":
                    CheckCase(function, "Pow");
                    CheckExactArgumentCount(function, 2);
                    Result = Mathf.Pow(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])), ConversionUtility.Convert<float>(Evaluate(function.Expressions[1])));
                    break;

                case "round":
                    CheckCase(function, "Round");
                    CheckExactArgumentCount(function, 1);
                    //var rounding = (options & EvaluateOptions.RoundAwayFromZero) == EvaluateOptions.RoundAwayFromZero ? MidpointRounding.AwayFromZero : MidpointRounding.ToEven;
                    Result = Mathf.Round(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "sign":
                    CheckCase(function, "Sign");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Sign(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));

                    break;

                case "sin":
                    CheckCase(function, "Sin");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Sin(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));
                    break;

                case "sqrt":
                    CheckCase(function, "Sqrt");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Sqrt(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));

                    break;

                case "tan":
                    CheckCase(function, "Tan");
                    CheckExactArgumentCount(function, 1);
                    Result = Mathf.Tan(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])));

                    break;

                case "max":
                    CheckCase(function, "Max");
                    CheckExactArgumentCount(function, 2);
                    Result = Mathf.Max(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])), ConversionUtility.Convert<float>(Evaluate(function.Expressions[1])));
                    break;

                case "min":
                    CheckCase(function, "Min");
                    CheckExactArgumentCount(function, 2);
                    Result = Mathf.Min(ConversionUtility.Convert<float>(Evaluate(function.Expressions[0])), ConversionUtility.Convert<float>(Evaluate(function.Expressions[1])));
                    break;

                case "in":
                    CheckCase(function, "In");
                    CheckExactArgumentCount(function, 2);

                    var parameter = Evaluate(function.Expressions[0]);

                    var evaluation = false;

                    // Goes through any values, and stop whe one is found
                    for (var i = 1; i < function.Expressions.Length; i++)
                    {
                        var argument = Evaluate(function.Expressions[i]);

                        if (Equals(parameter, argument))
                        {
                            evaluation = true;
                            break;
                        }
                    }

                    Result = evaluation;
                    break;

                default:
                    throw new ArgumentException("Function not found", function.Identifier.Name);
            }
        }

        private void CheckCase(FunctionExpression function, string reference)
        {
            var called = function.Identifier.Name;

            if (IgnoreCase)
            {
                if (string.Equals(called, reference, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                throw new ArgumentException("Function not found.", called);
            }

            if (called != reference)
            {
                throw new ArgumentException($"Function not found: '{called}'. Try '{reference}' instead.");
            }
        }

        private void OnEvaluateFunction(string name, FunctionArgs args)
        {
            EvaluateFunction?.Invoke(flow, name, args);
        }

        public override void Visit(IdentifierExpression identifier)
        {
            if (Parameters.ContainsKey(identifier.Name))
            {
                // The parameter is defined in the dictionary
                if (Parameters[identifier.Name] is Expression)
                {
                    // The parameter is itself another Expression
                    var expression = (Expression)Parameters[identifier.Name];

                    // Overloads parameters
                    foreach (var p in Parameters)
                    {
                        expression.Parameters[p.Key] = p.Value;
                    }

                    expression.EvaluateFunction += EvaluateFunction;
                    expression.EvaluateParameter += EvaluateParameter;

                    Result = ((Expression)Parameters[identifier.Name]).Evaluate(flow);
                }
                else
                {
                    Result = Parameters[identifier.Name];
                }
            }
            else
            {
                // The parameter should be defined in a callback method
                var args = new ParameterArgs();

                // Calls external implementation
                OnEvaluateParameter(identifier.Name, args);

                if (!args.HasResult)
                {
                    throw new ArgumentException("Parameter was not defined", identifier.Name);
                }

                Result = args.Result;
            }
        }

        private void OnEvaluateParameter(string name, ParameterArgs args)
        {
            EvaluateParameter?.Invoke(flow, name, args);
        }

        public static void CheckExactArgumentCount(FunctionExpression function, int count)
        {
            if (function.Expressions.Length != count)
            {
                throw new ArgumentException($"{function.Identifier.Name}() takes at exactly {count} arguments. {function.Expressions.Length} provided.");
            }
        }

        public static void CheckMinArgumentCount(FunctionExpression function, int count)
        {
            if (function.Expressions.Length < count)
            {
                throw new ArgumentException($"{function.Identifier.Name}() takes at at least {count} arguments. {function.Expressions.Length} provided.");
            }
        }

        private delegate T Func<T>();
    }
}
