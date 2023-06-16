using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class OverrideStack<T>
    {
        public OverrideStack(T defaultValue)
        {
            _value = defaultValue;
            getValue = () => _value;
            setValue = (value) => _value = value;
        }

        public OverrideStack(Func<T> getValue, Action<T> setValue)
        {
            Ensure.That(nameof(getValue)).IsNotNull(getValue);
            Ensure.That(nameof(setValue)).IsNotNull(setValue);

            this.getValue = getValue;
            this.setValue = setValue;
        }

        public OverrideStack(Func<T> getValue, Action<T> setValue, Action clearValue) : this(getValue, setValue)
        {
            Ensure.That(nameof(clearValue)).IsNotNull(clearValue);

            this.clearValue = clearValue;
        }

        private readonly Func<T> getValue;

        private readonly Action<T> setValue;

        private readonly Action clearValue;

        private T _value;

        private readonly Stack<T> previous = new Stack<T>();

        public T value
        {
            get
            {
                return getValue();
            }
            internal set
            {
                setValue(value);
            }
        }

        public OverrideLayer<T> Override(T item)
        {
            return new OverrideLayer<T>(this, item);
        }

        public void BeginOverride(T item)
        {
            previous.Push(value);
            value = item;
        }

        public void EndOverride()
        {
            if (previous.Count == 0)
            {
                throw new InvalidOperationException();
            }

            value = previous.Pop();

            if (previous.Count == 0)
            {
                clearValue?.Invoke();
            }
        }

        public static implicit operator T(OverrideStack<T> stack)
        {
            Ensure.That(nameof(stack)).IsNotNull(stack);

            return stack.value;
        }
    }

    public struct OverrideLayer<T> : IDisposable
    {
        public OverrideStack<T> stack { get; }

        internal OverrideLayer(OverrideStack<T> stack, T item)
        {
            Ensure.That(nameof(stack)).IsNotNull(stack);

            this.stack = stack;

            stack.BeginOverride(item);
        }

        public void Dispose()
        {
            stack.EndOverride();
        }
    }
}
