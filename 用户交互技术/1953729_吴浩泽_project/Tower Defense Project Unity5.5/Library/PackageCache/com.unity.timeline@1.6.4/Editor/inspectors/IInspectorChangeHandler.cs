namespace UnityEditor.Timeline
{
    /// <summary>
    /// Implement this interface in your PlayableAsset inspector to change what happens when a UI component in the inspector is modified
    /// </summary>
    /// <remarks>The default PlayableAsset inspector will cause any UI change to force a PlayableGraph rebuild</remarks>
    public interface IInspectorChangeHandler
    {
        /// <summary>
        /// This method will be called when a Playable Asset inspector is modified.
        /// </summary>
        void OnPlayableAssetChangedInInspector();
    }
}
