using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [Serializable]
    public struct SerializationData
    {
        [SerializeField]
        private string _json;

        public string json => _json;

        [SerializeField]
        private UnityObject[] _objectReferences;

        public UnityObject[] objectReferences => _objectReferences;

#if DEBUG_SERIALIZATION
        [SerializeField]
        private string _guid;

        public string guid => _guid;
#endif

        public SerializationData(string json, IEnumerable<UnityObject> objectReferences)
        {
            _json = json;
            _objectReferences = objectReferences?.ToArray() ?? Empty<UnityObject>.array;

#if DEBUG_SERIALIZATION
            _guid = Guid.NewGuid().ToString();
#endif
        }

        public SerializationData(string json, params UnityObject[] objectReferences) : this(json, ((IEnumerable<UnityObject>)objectReferences)) { }

        public string ToString(string title)
        {
            using (var writer = new StringWriter())
            {
                if (!string.IsNullOrEmpty(title))
                {
#if DEBUG_SERIALIZATION
                    writer.WriteLine(title + $" ({guid})");
#else

                    writer.WriteLine(title);
#endif
                    writer.WriteLine();
                }
#if DEBUG_SERIALIZATION
                else
                {
                    writer.WriteLine(guid);
                    writer.WriteLine();
                }
#endif

                writer.WriteLine("Object References: ");

                if (objectReferences.Length == 0)
                {
                    writer.WriteLine("(None)");
                }
                else
                {
                    var index = 0;

                    foreach (var objectReference in objectReferences)
                    {
                        if (objectReference.IsUnityNull())
                        {
                            writer.WriteLine($"{index}: null");
                        }
                        else if (UnityThread.allowsAPI)
                        {
                            writer.WriteLine($"{index}: {objectReference.GetType().FullName} [{objectReference.GetHashCode()}] \"{objectReference.name}\"");
                        }
                        else
                        {
                            writer.WriteLine($"{index}: {objectReference.GetType().FullName} [{objectReference.GetHashCode()}]");
                        }

                        index++;
                    }
                }

                writer.WriteLine();
                writer.WriteLine("JSON: ");
                writer.WriteLine(Serialization.PrettyPrint(json));

                return writer.ToString();
            }
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public void ShowString(string title = null)
        {
            var dataPath = Path.GetTempPath() + Guid.NewGuid() + ".json";
            File.WriteAllText(dataPath, ToString(title));
            Process.Start(dataPath);
        }
    }
}
