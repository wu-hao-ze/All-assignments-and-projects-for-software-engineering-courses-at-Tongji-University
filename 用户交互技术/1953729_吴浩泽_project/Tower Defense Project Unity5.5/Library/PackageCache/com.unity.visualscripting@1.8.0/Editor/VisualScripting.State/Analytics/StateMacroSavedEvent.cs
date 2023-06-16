using UnityEditor;

namespace Unity.VisualScripting.Analytics
{
    class StateMacroSavedEvent : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (assetType == typeof(StateGraphAsset))
                {
                    UsageAnalytics.CollectAndSend();
                    break;
                }
            }
            return paths;
        }
    }
}
