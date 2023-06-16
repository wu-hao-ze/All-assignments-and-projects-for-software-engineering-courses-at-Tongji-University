using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IMultiInputUnit : IUnit
    {
        int inputCount { get; set; }

        ReadOnlyCollection<ValueInput> multiInputs { get; }
    }

    public abstract class MultiInputUnit<T> : Unit, IMultiInputUnit
    {
        [SerializeAs(nameof(inputCount))]
        private int _inputCount = 2;

        [DoNotSerialize]
        protected virtual int minInputCount => 2;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Inputs")]
        public virtual int inputCount
        {
            get
            {
                return _inputCount;
            }
            set
            {
                _inputCount = Mathf.Clamp(value, minInputCount, 10);
            }
        }

        [DoNotSerialize]
        public ReadOnlyCollection<ValueInput> multiInputs { get; protected set; }

        protected override void Definition()
        {
            var _multiInputs = new List<ValueInput>();

            multiInputs = _multiInputs.AsReadOnly();

            for (var i = 0; i < inputCount; i++)
            {
                _multiInputs.Add(ValueInput<T>(i.ToString()));
            }
        }

        protected void InputsAllowNull()
        {
            foreach (var input in multiInputs)
            {
                input.AllowsNull();
            }
        }
    }
}
