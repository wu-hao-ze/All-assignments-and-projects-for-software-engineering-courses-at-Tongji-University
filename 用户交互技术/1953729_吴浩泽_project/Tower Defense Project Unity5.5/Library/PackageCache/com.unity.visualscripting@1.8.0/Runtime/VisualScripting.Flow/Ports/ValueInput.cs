using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public sealed class ValueInput : UnitPort<ValueOutput, IUnitOutputPort, ValueConnection>, IUnitValuePort, IUnitInputPort
    {
        public ValueInput(string key, Type type) : base(key)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            this.type = type;
        }

        public Type type { get; }

        public bool hasDefaultValue => unit.defaultValues.ContainsKey(key);

        public override IEnumerable<ValueConnection> validConnections => unit?.graph?.valueConnections.WithDestination(this) ?? Enumerable.Empty<ValueConnection>();

        public override IEnumerable<InvalidConnection> invalidConnections => unit?.graph?.invalidConnections.WithDestination(this) ?? Enumerable.Empty<InvalidConnection>();

        public override IEnumerable<ValueOutput> validConnectedPorts => validConnections.Select(c => c.source);

        public override IEnumerable<IUnitOutputPort> invalidConnectedPorts => invalidConnections.Select(c => c.source);

        // Use for inspector metadata
        [DoNotSerialize]
        internal object _defaultValue
        {
            get
            {
                return unit.defaultValues[key];
            }
            set
            {
                unit.defaultValues[key] = value;
            }
        }

        public bool nullMeansSelf { get; private set; }

        public bool allowsNull { get; private set; }

        public ValueConnection connection => unit.graph?.valueConnections.SingleOrDefaultWithDestination(this);

        public override bool hasValidConnection => connection != null;

        public void SetDefaultValue(object value)
        {
            Ensure.That(nameof(value)).IsOfType(value, type);

            if (!SupportsDefaultValue(type))
            {
                return;
            }

            if (unit.defaultValues.ContainsKey(key))
            {
                unit.defaultValues[key] = value;
            }
            else
            {
                unit.defaultValues.Add(key, value);
            }
        }

        public override bool CanConnectToValid(ValueOutput port)
        {
            var source = port;
            var destination = this;

            return source.type.IsConvertibleTo(destination.type, false);
        }

        public override void ConnectToValid(ValueOutput port)
        {
            var source = port;
            var destination = this;

            destination.Disconnect();

            unit.graph.valueConnections.Add(new ValueConnection(source, destination));
        }

        public override void ConnectToInvalid(IUnitOutputPort port)
        {
            ConnectInvalid(port, this);
        }

        public override void DisconnectFromValid(ValueOutput port)
        {
            var connection = validConnections.SingleOrDefault(c => c.source == port);

            if (connection != null)
            {
                unit.graph.valueConnections.Remove(connection);
            }
        }

        public override void DisconnectFromInvalid(IUnitOutputPort port)
        {
            DisconnectInvalid(port, this);
        }

        public ValueInput NullMeansSelf()
        {
            if (ComponentHolderProtocol.IsComponentHolderType(type))
            {
                nullMeansSelf = true;
            }

            return this;
        }

        public ValueInput AllowsNull()
        {
            if (type.IsNullable())
            {
                allowsNull = true;
            }

            return this;
        }

        private static readonly HashSet<Type> typesWithDefaultValues = new HashSet<Type>()
        {
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Color),
            typeof(AnimationCurve),
            typeof(Rect),
            typeof(Ray),
            typeof(Ray2D),
            typeof(Type),
#if PACKAGE_INPUT_SYSTEM_EXISTS
            typeof(UnityEngine.InputSystem.InputAction),
#endif
        };

        public static bool SupportsDefaultValue(Type type)
        {
            return
                typesWithDefaultValues.Contains(type) ||
                typesWithDefaultValues.Contains(Nullable.GetUnderlyingType(type)) ||
                type.IsBasic() ||
                typeof(UnityObject).IsAssignableFrom(type);
        }

        public override IUnitPort CompatiblePort(IUnit unit)
        {
            if (unit == this.unit) return null;

            return unit.CompatibleValueOutput(type);
        }
    }
}
