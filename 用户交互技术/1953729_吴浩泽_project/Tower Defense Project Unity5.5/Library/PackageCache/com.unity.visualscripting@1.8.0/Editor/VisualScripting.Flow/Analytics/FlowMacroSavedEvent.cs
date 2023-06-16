using UnityEditor;

namespace Unity.VisualScripting.Analytics
{
    class FlowMacroSavedEvent : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (assetType == typeof(ScriptGraphAsset))
                {
                    UsageAnalytics.CollectAndSend();
                    break;
                }
            }
            return paths;
        }
    }
}
