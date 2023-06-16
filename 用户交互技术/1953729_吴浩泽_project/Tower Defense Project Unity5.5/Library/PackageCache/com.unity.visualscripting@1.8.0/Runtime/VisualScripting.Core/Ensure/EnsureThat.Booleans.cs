using System;

namespace Unity.VisualScripting
{
    public partial class EnsureThat
    {
        public void IsTrue(bool value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!value)
            {
                throw new ArgumentException(ExceptionMessages.Booleans_IsTrueFailed, paramName);
            }
        }

        public void IsFalse(bool value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value)
            {
                throw new ArgumentException(ExceptionMessages.Booleans_IsFalseFailed, paramName);
            }
        }
    }
}
