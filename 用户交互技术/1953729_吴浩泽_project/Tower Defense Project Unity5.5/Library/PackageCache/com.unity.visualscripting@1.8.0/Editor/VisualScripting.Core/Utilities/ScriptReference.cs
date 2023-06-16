using System;

namespace Unity.VisualScripting
{
    public struct ScriptReference
    {
        /// <summary>
        /// The ID of the script in the source file.
        /// </summary>
        public int fileID;

        /// <summary>
        /// The GUID of the source file (script or DLL).
        /// </summary>
        public string guid;

        private ScriptReference(string guid, int fileID)
        {
            this.guid = guid;
            this.fileID = fileID;
        }

        public static ScriptReference Existing(Type type)
        {
            return new ScriptReference(ScriptUtility.GetScriptGuid(type), ScriptUtility.GetFileID(type));
        }

        public static ScriptReference Manual(string guid, int fileID)
        {
            return new ScriptReference(guid, fileID);
        }

        public static ScriptReference Cs(string csGuid)
        {
            return new ScriptReference(csGuid, ScriptUtility.CsFileID);
        }

        public static ScriptReference Dll(string dllGuid, Type type)
        {
            return new ScriptReference(dllGuid, ScriptUtility.GetDllFileID(type));
        }

        public static ScriptReference Dll(string dllGuid, string @namespace, string typeName)
        {
            return new ScriptReference(dllGuid, ScriptUtility.GetDllFileID(@namespace, typeName));
        }

        public override string ToString()
        {
            return $"{{fileID: {fileID}, guid: {guid}, type: 3}}";
        }
    }
}
