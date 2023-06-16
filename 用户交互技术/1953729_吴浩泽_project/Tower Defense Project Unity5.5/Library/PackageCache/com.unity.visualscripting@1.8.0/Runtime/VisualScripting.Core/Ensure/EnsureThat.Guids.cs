using System;

namespace Unity.VisualScripting
{
    public partial class EnsureThat
    {
        public void IsNotEmpty(Guid value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value.Equals(Guid.Empty))
            {
                throw new ArgumentException(ExceptionMessages.Guids_IsNotEmpty_Failed, paramName);
            }
        }
    }
}
