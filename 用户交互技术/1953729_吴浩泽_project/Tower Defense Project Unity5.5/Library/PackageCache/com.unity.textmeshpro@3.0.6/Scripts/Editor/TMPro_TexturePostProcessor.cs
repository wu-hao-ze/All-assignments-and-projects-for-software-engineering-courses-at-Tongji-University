using System;
using UnityEngine;
using UnityEditor;


namespace TMPro.EditorUtilities
{
    /// <summary>
    /// Asset post processor used to handle text assets changes.
    /// This includes tracking of changes to textures used by sprite assets as well as font assets potentially getting updated outside of the Unity editor.
    /// </summary>
    internal class TMPro_TexturePostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var asset in importedAssets)
            {
                // Return if imported asset path is outside of the project.
                if (asset.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                Type assetType = AssetDatabase.GetMainAssetTypeAtPath(asset);

                if (assetType == typeof(TMP_FontAsset))
                {
                    TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath(asset, typeof(TMP_FontAsset)) as TMP_FontAsset;

                    // Only refresh font asset definition if font asset was previously initialized.
                    if (fontAsset != null && fontAsset.m_CharacterLookupDictionary != null)
                        TMP_EditorResourceManager.RegisterFontAssetForDefinitionRefresh(fontAsset);
                }

                if (assetType == typeof(Texture2D))
                {
                    Texture2D tex = AssetDatabase.LoadAssetAtPath(asset, typeof(Texture2D)) as Texture2D;

                    if (tex == null)
                        continue;

                    TMPro_EventManager.ON_SPRITE_ASSET_PROPERTY_CHANGED(true, tex);
                    Resources.UnloadAsset(tex);
                }
            }
        }
    }
}
