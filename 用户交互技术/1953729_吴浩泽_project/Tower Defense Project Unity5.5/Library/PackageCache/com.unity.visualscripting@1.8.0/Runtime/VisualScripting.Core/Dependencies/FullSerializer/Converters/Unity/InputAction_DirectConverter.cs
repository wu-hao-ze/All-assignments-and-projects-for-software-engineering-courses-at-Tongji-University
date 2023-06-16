#if !NO_UNITY
#if PACKAGE_INPUT_SYSTEM_EXISTS

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.InputSystem;

namespace Unity.VisualScripting.FullSerializer
{
    partial class fsConverterRegistrar
    {
        [UsedImplicitly]
        public static InputAction_DirectConverter Register_InputAction_DirectConverter;
    }

    [UsedImplicitly]
    public class InputAction_DirectConverter : fsDirectConverter<InputAction>
    {
        protected override fsResult DoSerialize(InputAction model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "id", model.id.ToString());
            result += SerializeMember(serialized, null, "name", model.name.ToString());
            result += SerializeMember(serialized, null, "expectedControlType", model.expectedControlType);
            result += SerializeMember(serialized, null, "type", model.type);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref InputAction model)
        {
            var result = fsResult.Success;

            result += DeserializeMember(data, null, "id", out string actionId);
            result += DeserializeMember(data, null, "name", out string actionName);
            result += DeserializeMember(data, null, "expectedControlType", out string expectedControlType);
            result += DeserializeMember(data, null, "type", out InputActionType type);

            model = MakeInputActionWithId(actionId, actionName, expectedControlType, type);

            return result;
        }

        /// <summary>
        /// Creates a fake InputAction. Ports with an editor MUST serialize a value of the port's type, even if a GUID
        /// would suffice in that case
        /// </summary>
        public static InputAction MakeInputActionWithId(string actionId, string actionName, string expectedControlType, InputActionType type)
        {
            var model = new InputAction();
            typeof(InputAction).GetField("m_Id", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(model, actionId);
            typeof(InputAction).GetField("m_Name", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(model, actionName);
            typeof(InputAction).GetField("m_Type", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(model, type);
            model.expectedControlType = expectedControlType;
            return model;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new InputAction();
        }
    }
}

#endif
#endif
