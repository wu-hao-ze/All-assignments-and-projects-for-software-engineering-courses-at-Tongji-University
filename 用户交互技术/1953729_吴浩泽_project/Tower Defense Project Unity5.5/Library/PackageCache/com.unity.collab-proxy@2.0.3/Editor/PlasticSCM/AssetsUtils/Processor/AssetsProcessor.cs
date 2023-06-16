using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;

namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    internal static class AssetsProcessors
    {
        internal static void Enable(
            string wkPath,
            PlasticAssetsProcessor plasticAssetsProcessor,
            IAssetStatusCache assetStatusCache)
        {
            AssetPostprocessor.Enable(wkPath, plasticAssetsProcessor);
            AssetModificationProcessor.Enable(wkPath, assetStatusCache);
        }

        internal static void Disable()
        {
            AssetPostprocessor.Disable();
            AssetModificationProcessor.Disable();
        }
    }
}
