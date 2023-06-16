using System;

namespace Unity.VisualScripting
{
    public partial class EnsureThat
    {
        public void Is<T>(T param, T expected) where T : struct, IComparable<T>
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!param.IsEq(expected))
            {
                throw new ArgumentException(ExceptionMessages.Comp_Is_Failed.Inject(param, expected), paramName);
            }
        }

        public void IsNot<T>(T param, T expected) where T : struct, IComparable<T>
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (param.IsEq(expected))
            {
                throw new ArgumentException(ExceptionMessages.Comp_IsNot_Failed.Inject(param, expected), paramName);
            }
        }

        public void IsLt<T>(T param, T limit) where T : struct, IComparable<T>
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!param.IsLt(limit))
            {
                throw new ArgumentException(ExceptionMessages.Comp_IsNotLt.Inject(param, limit), paramName);
            }
        }

        public void IsLte<T>(T param, T limit) where T : struct, IComparable<T>
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (param.IsGt(limit))
            {
                throw new ArgumentException(ExceptionMessages.Comp_IsNotLte.Inject(param, limit), paramName);
            }
        }

        public void IsGt<T>(T param, T limit) where T : struct, IComparable<T>
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!param.IsGt(limit))
            {
                throw new ArgumentException(ExceptionMessages.Comp_IsNotGt.Inject(param, limit), paramName);
            }
        }

        public void IsGte<T>(T param, T limit) where T : struct, IComparable<T>
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (param.IsLt(limit))
            {
                throw new ArgumentException(ExceptionMessages.Comp_IsNotGte.Inject(param, limit), paramName);
            }
        }

        public void IsInRange<T>(T param, T min, T max) where T : struct, IComparable<T>
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (param.IsLt(min))
            {
                throw new ArgumentException(ExceptionMessages.Comp_IsNotInRange_ToLow.Inject(param, min), paramName);
            }

            if (param.IsGt(max))
            {
                throw new ArgumentException(ExceptionMessages.Comp_IsNotInRange_ToHigh.Inject(param, max), paramName);
            }
        }
    }
}
