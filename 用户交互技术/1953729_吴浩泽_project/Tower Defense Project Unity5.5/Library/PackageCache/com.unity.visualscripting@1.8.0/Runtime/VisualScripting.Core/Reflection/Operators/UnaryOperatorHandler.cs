using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class UnaryOperatorHandler : OperatorHandler
    {
        protected UnaryOperatorHandler(string name, string verb, string symbol, string customMethodName)
            : base(name, verb, symbol, customMethodName) { }

        private readonly Dictionary<Type, Func<object, object>> manualHandlers = new Dictionary<Type, Func<object, object>>();
        private readonly Dictionary<Type, IOptimizedInvoker> userDefinedOperators = new Dictionary<Type, IOptimizedInvoker>();
        private readonly Dictionary<Type, Type> userDefinedOperandTypes = new Dictionary<Type, Type>();

        public object Operate(object operand)
        {
            Ensure.That(nameof(operand)).IsNotNull(operand);

            var type = operand.GetType();

            if (manualHandlers.ContainsKey(type))
            {
                return manualHandlers[type](operand);
            }

            if (customMethodName != null)
            {
                if (!userDefinedOperators.ContainsKey(type))
                {
                    var method = type.GetMethod(customMethodName, BindingFlags.Public | BindingFlags.Static);

                    if (method != null)
                    {
                        userDefinedOperandTypes.Add(type, ResolveUserDefinedOperandType(method));
                    }

                    userDefinedOperators.Add(type, method?.Prewarm());
                }

                if (userDefinedOperators[type] != null)
                {
                    operand = ConversionUtility.Convert(operand, userDefinedOperandTypes[type]);

                    return userDefinedOperators[type].Invoke(null, operand);
                }
            }

            return CustomHandling(operand);
        }

        protected virtual object CustomHandling(object operand)
        {
            throw new InvalidOperatorException(symbol, operand.GetType());
        }

        protected void Handle<T>(Func<T, object> handler)
        {
            manualHandlers.Add(typeof(T), operand => handler((T)operand));
        }

        private static Type ResolveUserDefinedOperandType(MethodInfo userDefinedOperator)
        {
            // See comment in BinaryOperatorHandler
            return userDefinedOperator.GetParameters()[0].ParameterType;
        }
    }
}
