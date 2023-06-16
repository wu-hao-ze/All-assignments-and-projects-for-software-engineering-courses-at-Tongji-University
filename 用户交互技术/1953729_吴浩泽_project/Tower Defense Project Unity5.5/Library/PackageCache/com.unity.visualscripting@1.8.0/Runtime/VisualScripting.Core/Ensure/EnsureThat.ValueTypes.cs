using System;

namespace Unity.VisualScripting
{
    public partial class EnsureThat
    {
        public void IsNotDefault<T>(T param) where T : struct
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (default(T).Equals(param))
            {
                throw new ArgumentException(ExceptionMessages.ValueTypes_IsNotDefault_Failed, paramName);
            }
        }
    }
}
