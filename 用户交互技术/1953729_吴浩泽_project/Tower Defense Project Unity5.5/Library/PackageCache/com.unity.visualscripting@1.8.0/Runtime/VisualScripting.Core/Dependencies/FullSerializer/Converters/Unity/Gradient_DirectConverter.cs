#if !NO_UNITY
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting.FullSerializer
{
    partial class fsConverterRegistrar
    {
        public static Gradient_DirectConverter Register_Gradient_DirectConverter;
    }

    public class Gradient_DirectConverter : fsDirectConverter<Gradient>
    {
        protected override fsResult DoSerialize(Gradient model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "alphaKeys", model.alphaKeys);
            result += SerializeMember(serialized, null, "colorKeys", model.colorKeys);

            try
            {
                result += SerializeMember(serialized, null, "mode", model.mode);
            }
            catch (Exception)
            {
                LogWarning("serialized");
            }

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Gradient model)
        {
            var result = fsResult.Success;

            var t0 = model.alphaKeys;
            result += DeserializeMember(data, null, "alphaKeys", out t0);
            model.alphaKeys = t0;

            var t1 = model.colorKeys;
            result += DeserializeMember(data, null, "colorKeys", out t1);
            model.colorKeys = t1;

            try
            {
                var t2 = model.mode;
                result += DeserializeMember(data, null, "mode", out t2);
                model.mode = t2;
            }
            catch (Exception)
            {
                LogWarning("deserialized");
            }

            return result;
        }

        static void LogWarning(string phase)
        {
            var fixedVersion = "2021.3.9f1";

#if UNITY_2022_2_OR_NEWER
            fixedVersion = "2022.2.0a18";
#elif UNITY_2022_1_OR_NEWER
            fixedVersion = "2022.1.9f1";
#endif

            Debug.LogWarning($"Gradient.mode could not be {phase}. Please use Unity {fixedVersion} or newer to resolve this issue.");
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new Gradient();
        }
    }
}
#endif
