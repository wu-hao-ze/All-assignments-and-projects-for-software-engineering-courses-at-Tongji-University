using System;

namespace Unity.VisualScripting
{
    public partial class EnsureThat
    {
        public void IsOfType<T>(T param, Type expectedType)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!expectedType.IsAssignableFrom(param))
            {
                throw new ArgumentException(ExceptionMessages.Types_IsOfType_Failed.Inject(expectedType.ToString(), param?.GetType().ToString() ?? "null"), paramName);
            }
        }

        public void IsOfType(Type param, Type expectedType)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!expectedType.IsAssignableFrom(param))
            {
                throw new ArgumentException(ExceptionMessages.Types_IsOfType_Failed.Inject(expectedType.ToString(), param.ToString()), paramName);
            }
        }

        public void IsOfType<T>(object param) => IsOfType(param, typeof(T));

        public void IsOfType<T>(Type param) => IsOfType(param, typeof(T));
    }
}
