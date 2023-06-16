using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.VisualScripting
{
    static class MovedFromAttributeExtensions
    {
        static Type s_DataType;
        static FieldInfo s_DataFieldInfo;
        static FieldInfo s_AutoUpdateAPIFieldInfo;
        static FieldInfo s_NamespaceFieldInfo;
        static FieldInfo s_AssemblyFieldInfo;
        static FieldInfo s_ClassNameFieldInfo;

        public static void GetData(
            this MovedFromAttribute @this,
            out bool autoUpdateAPI,
            out string sourceNamespace,
            out string sourceAssembly,
            out string sourceClassName)
        {
            Initialize();

            autoUpdateAPI = false;
            sourceNamespace = string.Empty;
            sourceAssembly = string.Empty;
            sourceClassName = string.Empty;

            if (s_DataFieldInfo != null &&
                s_AutoUpdateAPIFieldInfo != null &&
                s_NamespaceFieldInfo != null &&
                s_AssemblyFieldInfo != null &&
                s_ClassNameFieldInfo != null)
            {
                var data = s_DataFieldInfo.GetValue(@this);
                autoUpdateAPI = (bool)s_AutoUpdateAPIFieldInfo.GetValue(data);
                sourceNamespace = (string)s_NamespaceFieldInfo.GetValue(data);
                sourceAssembly = (string)s_AssemblyFieldInfo.GetValue(data);
                sourceClassName = (string)s_ClassNameFieldInfo.GetValue(data);
            }
        }

        static void Initialize()
        {
            if (s_DataType == null)
            {
                s_DataType = typeof(MovedFromAttribute).Assembly.GetType("UnityEngine.Scripting.APIUpdating.MovedFromAttributeData");
                s_DataFieldInfo = typeof(MovedFromAttribute).GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);

                if (s_DataType != null)
                {
                    s_AutoUpdateAPIFieldInfo = s_DataType.GetField("autoUdpateAPI");
                    s_NamespaceFieldInfo = s_DataType.GetField("nameSpace");
                    s_AssemblyFieldInfo = s_DataType.GetField("assembly");
                    s_ClassNameFieldInfo = s_DataType.GetField("className");
                }
            }
        }
    }
}
