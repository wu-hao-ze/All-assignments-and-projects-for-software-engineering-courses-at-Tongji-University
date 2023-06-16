using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class BinaryOperatorHandler : OperatorHandler
    {
        protected BinaryOperatorHandler(string name, string verb, string symbol, string customMethodName)
            : base(name, verb, symbol, customMethodName) { }

        private readonly Dictionary<OperatorQuery, Func<object, object, object>> handlers = new Dictionary<OperatorQuery, Func<object, object, object>>();
        private readonly Dictionary<OperatorQuery, IOptimizedInvoker> userDefinedOperators = new Dictionary<OperatorQuery, IOptimizedInvoker>();
        private readonly Dictionary<OperatorQuery, OperatorQuery> userDefinedOperandTypes = new Dictionary<OperatorQuery, OperatorQuery>();

        public virtual object Operate(object leftOperand, object rightOperand)
        {
            OperatorQuery query;

            var leftType = leftOperand?.GetType();
            var rightType = rightOperand?.GetType();

            if (leftType != null && rightType != null)
            {
                query = new OperatorQuery(leftType, rightType);
            }
            else if (leftType != null && leftType.IsNullable())
            {
                query = new OperatorQuery(leftType, leftType);
            }
            else if (rightType != null && rightType.IsNullable())
            {
                query = new OperatorQuery(rightType, rightType);
            }
            else if (leftType == null && rightType == null)
            {
                return BothNullHandling();
            }
            else
            {
                return SingleNullHandling();
            }

            if (handlers.ContainsKey(query))
            {
                return handlers[query](leftOperand, rightOperand);
            }

            if (customMethodName != null)
            {
                if (!userDefinedOperators.ContainsKey(query))
                {
                    var leftMethod = query.leftType.GetMethod(customMethodName, BindingFlags.Public | BindingFlags.Static, null, new[] { query.leftType, query.rightType }, null);

                    if (query.leftType != query.rightType)
                    {
                        var rightMethod = query.rightType.GetMethod(customMethodName, BindingFlags.Public | BindingFlags.Static, null, new[] { query.leftType, query.rightType }, null);

                        if (leftMethod != null && rightMethod != null)
                        {
                            throw new AmbiguousOperatorException(symbol, query.leftType, query.rightType);
                        }

                        var method = (leftMethod ?? rightMethod);

                        if (method != null)
                        {
                            userDefinedOperandTypes.Add(query, ResolveUserDefinedOperandTypes(method));
                        }

                        userDefinedOperators.Add(query, method?.Prewarm());
                    }
                    else
                    {
                        if (leftMethod != null)
                        {
                            userDefinedOperandTypes.Add(query, ResolveUserDefinedOperandTypes(leftMethod));
                        }

                        userDefinedOperators.Add(query, leftMethod?.Prewarm());
                    }
                }

                if (userDefinedOperators[query] != null)
                {
                    leftOperand = ConversionUtility.Convert(leftOperand, userDefinedOperandTypes[query].leftType);
                    rightOperand = ConversionUtility.Convert(rightOperand, userDefinedOperandTypes[query].rightType);

                    return userDefinedOperators[query].Invoke(null, leftOperand, rightOperand);
                }
            }

            return CustomHandling(leftOperand, rightOperand);
        }

        protected virtual object CustomHandling(object leftOperand, object rightOperand)
        {
            throw new InvalidOperatorException(symbol, leftOperand?.GetType(), rightOperand?.GetType());
        }

        protected virtual object BothNullHandling()
        {
            throw new InvalidOperatorException(symbol, null, null);
        }

        protected virtual object SingleNullHandling()
        {
            throw new InvalidOperatorException(symbol, null, null);
        }

        protected void Handle<TLeft, TRight>(Func<TLeft, TRight, object> handler, bool reverse = false)
        {
            var query = new OperatorQuery(typeof(TLeft), typeof(TRight));

            if (handlers.ContainsKey(query))
            {
                throw new ArgumentException($"A handler is already registered for '{typeof(TLeft)} {symbol} {typeof(TRight)}'.");
            }

            handlers.Add(query, (left, right) => handler((TLeft)left, (TRight)right));

            if (reverse && typeof(TLeft) != typeof(TRight))
            {
                var reverseQuery = new OperatorQuery(typeof(TRight), typeof(TLeft));

                if (!handlers.ContainsKey(reverseQuery))
                {
                    handlers.Add(reverseQuery, (left, right) => handler((TLeft)left, (TRight)right));
                }
            }
        }

        private static OperatorQuery ResolveUserDefinedOperandTypes(MethodInfo userDefinedOperator)
        {
            // We will need to convert the operands to the argument types,
            // because .NET is actually permissive of implicit conversion
            // in its GetMethod calls. For example, an operator requiring
            // a float operand will accept an int. However, our optimized
            // reflection code is more strict, and will only accept the
            // exact type, hence why we need to manually store the expected
            // parameter types here to convert them later.
            var parameters = userDefinedOperator.GetParameters();
            return new OperatorQuery(parameters[0].ParameterType, parameters[1].ParameterType);
        }

        private struct OperatorQuery : IEquatable<OperatorQuery>
        {
            public readonly Type leftType;
            public readonly Type rightType;

            public OperatorQuery(Type leftType, Type rightType)
            {
                this.leftType = leftType;
                this.rightType = rightType;
            }

            public bool Equals(OperatorQuery other)
            {
                return
                    leftType == other.leftType &&
                    rightType == other.rightType;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is OperatorQuery))
                {
                    return false;
                }

                return Equals((OperatorQuery)obj);
            }

            public override int GetHashCode()
            {
                return HashUtility.GetHashCode(leftType, rightType);
            }
        }
    }
}
