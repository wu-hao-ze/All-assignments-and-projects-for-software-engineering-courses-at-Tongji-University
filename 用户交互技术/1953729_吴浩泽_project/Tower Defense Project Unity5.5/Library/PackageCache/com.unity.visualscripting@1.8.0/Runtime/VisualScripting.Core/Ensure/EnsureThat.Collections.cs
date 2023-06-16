using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public partial class EnsureThat
    {
        public void HasItems<T>(T value) where T : class, ICollection
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            IsNotNull(value);

            if (value.Count < 1)
            {
                throw new ArgumentException(ExceptionMessages.Collections_HasItemsFailed, paramName);
            }
        }

        public void HasItems<T>(ICollection<T> value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            IsNotNull(value);

            if (value.Count < 1)
            {
                throw new ArgumentException(ExceptionMessages.Collections_HasItemsFailed, paramName);
            }
        }

        public void HasItems<T>(T[] value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            IsNotNull(value);

            if (value.Length < 1)
            {
                throw new ArgumentException(ExceptionMessages.Collections_HasItemsFailed, paramName);
            }
        }

        public void HasNoNullItem<T>(T value) where T : class, IEnumerable
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            IsNotNull(value);

            foreach (var item in value)
            {
                if (item == null)
                {
                    throw new ArgumentException(ExceptionMessages.Collections_HasNoNullItemFailed, paramName);
                }
            }
        }

        public void HasItems<T>(IList<T> value) => HasItems(value as ICollection<T>);

        public void HasItems<TKey, TValue>(IDictionary<TKey, TValue> value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            IsNotNull(value);

            if (value.Count < 1)
            {
                throw new ArgumentException(ExceptionMessages.Collections_HasItemsFailed, paramName);
            }
        }

        public void SizeIs<T>(T[] value, int expected)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value.Length != expected)
            {
                throw new ArgumentException(ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, value.Length), paramName);
            }
        }

        public void SizeIs<T>(T[] value, long expected)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value.Length != expected)
            {
                throw new ArgumentException(ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, value.Length), paramName);
            }
        }

        public void SizeIs<T>(T value, int expected) where T : ICollection
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value.Count != expected)
            {
                throw new ArgumentException(ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, value.Count), paramName);
            }
        }

        public void SizeIs<T>(T value, long expected) where T : ICollection
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value.Count != expected)
            {
                throw new ArgumentException(ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, value.Count), paramName);
            }
        }

        public void SizeIs<T>(ICollection<T> value, int expected)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value.Count != expected)
            {
                throw new ArgumentException(ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, value.Count), paramName);
            }
        }

        public void SizeIs<T>(ICollection<T> value, long expected)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value.Count != expected)
            {
                throw new ArgumentException(ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, value.Count), paramName);
            }
        }

        public void SizeIs<T>(IList<T> value, int expected) => SizeIs(value as ICollection<T>, expected);

        public void SizeIs<T>(IList<T> value, long expected) => SizeIs(value as ICollection<T>, expected);

        public void SizeIs<TKey, TValue>(IDictionary<TKey, TValue> value, int expected)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value.Count != expected)
            {
                throw new ArgumentException(ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, value.Count), paramName);
            }
        }

        public void SizeIs<TKey, TValue>(IDictionary<TKey, TValue> value, long expected)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value.Count != expected)
            {
                throw new ArgumentException(ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, value.Count), paramName);
            }
        }

        public void IsKeyOf<TKey, TValue>(IDictionary<TKey, TValue> value, TKey expectedKey, string keyLabel = null)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!value.ContainsKey(expectedKey))
            {
                throw new ArgumentException(ExceptionMessages.Collections_ContainsKey_Failed.Inject(expectedKey, keyLabel ?? paramName.Prettify()), paramName);
            }
        }

        public void Any<T>(IList<T> value, Func<T, bool> predicate) => Any(value as ICollection<T>, predicate);

        public void Any<T>(ICollection<T> value, Func<T, bool> predicate)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!value.Any(predicate))
            {
                throw new ArgumentException(ExceptionMessages.Collections_Any_Failed, paramName);
            }
        }

        public void Any<T>(T[] value, Func<T, bool> predicate)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!value.Any(predicate))
            {
                throw new ArgumentException(ExceptionMessages.Collections_Any_Failed, paramName);
            }
        }
    }
}
