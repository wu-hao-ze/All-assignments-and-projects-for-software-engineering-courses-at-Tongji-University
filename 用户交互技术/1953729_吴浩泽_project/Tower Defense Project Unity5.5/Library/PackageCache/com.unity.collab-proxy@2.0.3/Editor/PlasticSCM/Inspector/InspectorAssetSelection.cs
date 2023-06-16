using UnityEditor.VersionControl;

using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetUtils;

namespace Unity.PlasticSCM.Editor.Inspector
{
    internal class InspectorAssetSelection : AssetOperations.IAssetSelection
    {
        AssetList AssetOperations.IAssetSelection.GetSelectedAssets()
        {
            return GetInspectorAssets(mActiveInspector);
        }

        internal void SetActiveInspector(UnityEditor.Editor inspector)
        {
            mActiveInspector = inspector;
        }

        static AssetList GetInspectorAssets(UnityEditor.Editor inspector)
        {
            AssetList result = new AssetList();

            if (inspector == null)
                return result;

            foreach (UnityEngine.Object obj in inspector.targets)
            {
                string assetPath = AssetsPath.GetFullPath.ForObject(obj);
                
                if (string.IsNullOrEmpty(assetPath))
                    continue;

                result.Add(new Asset(assetPath));
            }

            return result;
        }

        UnityEditor.Editor mActiveInspector;
    }
}
