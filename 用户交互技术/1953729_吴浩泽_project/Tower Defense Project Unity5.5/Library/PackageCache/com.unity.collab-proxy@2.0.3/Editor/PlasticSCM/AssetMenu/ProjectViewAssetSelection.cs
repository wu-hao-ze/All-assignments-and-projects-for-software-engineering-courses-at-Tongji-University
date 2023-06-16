using System;

using UnityEditor;
using UnityEditor.VersionControl;

using Unity.PlasticSCM.Editor.AssetUtils;

namespace Unity.PlasticSCM.Editor.AssetMenu
{
    internal class ProjectViewAssetSelection : AssetOperations.IAssetSelection
    {
        internal ProjectViewAssetSelection(Action assetSelectionChangedAction)
        {
            mAssetSelectionChangedAction = assetSelectionChangedAction;

            Selection.selectionChanged += SelectionChanged;
        }

        internal void Dispose()
        {
            Selection.selectionChanged -= SelectionChanged;
        }

        void SelectionChanged()
        {
            // Selection.selectionChanged gets triggered on both
            // project view and scene view. We only want to trigger
            // the action if user selects on project view (has assets)
            if (HasSelectedAssets())
                mAssetSelectionChangedAction();
        }

        AssetList AssetOperations.IAssetSelection.GetSelectedAssets()
        {
            if (Selection.assetGUIDs.Length == 0)
                return new AssetList();

            AssetList result = new AssetList();

            foreach (string guid in Selection.assetGUIDs)
            {
                string assetPath = AssetsPath.GetFullPath.ForGuid(guid);

                if (string.IsNullOrEmpty(assetPath))
                    continue;

                result.Add(new Asset(assetPath));
            }

            return result;
        }

        bool HasSelectedAssets()
        {
            // Objects in project view have GUIDs, objects in scene view don't
            return Selection.assetGUIDs.Length > 0;
        }

        Action mAssetSelectionChangedAction;
    }
}
