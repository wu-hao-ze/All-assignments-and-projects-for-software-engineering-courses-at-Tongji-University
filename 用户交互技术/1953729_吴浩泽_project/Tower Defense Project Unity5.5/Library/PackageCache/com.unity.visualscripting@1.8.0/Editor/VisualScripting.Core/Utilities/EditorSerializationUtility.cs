using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting.YamlDotNet.RepresentationModel;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class EditorSerializationUtility
    {
#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Log Stuck Serialization Dependers", priority = LudiqProduct.DeveloperToolsMenuPriority + 901)]
#endif
        public static void LogStuckDependers()
        {
            Serialization.LogStuckDependers();
        }

        // Instantly deserializes an asset on the current thread

        public static void DeserializeYamlAsset(UnityObject asset, string topNodeKey = "MonoBehaviour", string dataNodeKey = "_data")
        {
            Ensure.That(nameof(asset)).IsNotNull(asset);

            var data = DeserializeYamlAsset(AssetDatabase.GetAssetPath(asset), topNodeKey, dataNodeKey);

            var dataField = new Member(asset.GetType(), dataNodeKey, Type.EmptyTypes);

            dataField.Set(asset, data);

            (asset as ISerializationCallbackReceiver)?.OnAfterDeserialize();
        }

        public static SerializationData DeserializeYamlAsset(string asset, string topNodeKey = "MonoBehaviour", string dataNodeKey = "_data")
        {
            Ensure.That(nameof(asset)).IsNotNull(asset);
            Ensure.That(nameof(topNodeKey)).IsNotNull(topNodeKey);
            Ensure.That(nameof(dataNodeKey)).IsNotNull(dataNodeKey);

            var assetPath = Path.Combine(Paths.project, asset);

            if (!File.Exists(assetPath))
            {
                throw new FileNotFoundException("Asset file not found.", assetPath);
            }

            try
            {
                var input = new StreamReader(assetPath);

                var yaml = new YamlStream();

                yaml.Load(input);

                // Find the data node.
                var rootNode = (YamlMappingNode)yaml.Documents[0].RootNode;
                var topNode = (YamlMappingNode)rootNode.Children[topNodeKey];
                var dataNode = (YamlMappingNode)topNode.Children[dataNodeKey];
                var jsonNode = (YamlScalarNode)dataNode.Children["_json"];
                var objectReferencesNode = (YamlSequenceNode)dataNode.Children["_objectReferences"];

                // Read the contents
                var json = jsonNode.Value;

                var objectReferences = new List<UnityObject>();

                foreach (var objectReferenceNode in objectReferencesNode.Children.Cast<YamlScalarNode>())
                {
                    objectReferences.Add(EditorUtility.InstanceIDToObject(int.Parse(objectReferenceNode.Value)));
                }

                // Return the final serialization data
                return new SerializationData(json, objectReferences);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize YAML asset.", ex);
            }
        }
    }
}
