#if VISUAL_SCRIPT_INTERNAL
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundleCreator : MonoBehaviour
{
    //TODO: Create CI to automatically create and update the assetbundle

    [MenuItem("Tools/Visual Scripting/Internal/Build Asset Bundle")]
    private static void BuildAssetBundle()
    {
        BuildPipeline.BuildAssetBundles("Assets/", BuildAssetBundleOptions.None,
            BuildTarget.StandaloneOSX);
    }
}
#endif
