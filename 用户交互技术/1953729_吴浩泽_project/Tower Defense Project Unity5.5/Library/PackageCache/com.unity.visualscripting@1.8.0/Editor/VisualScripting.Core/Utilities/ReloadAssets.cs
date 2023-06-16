using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor.Callbacks;

//TODO: Remove when the asset bundle is fixed
public class ReloadAssets
{
    internal delegate void BuildCompleted();

    internal static BuildCompleted OnBuildCompleted;

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        OnBuildCompleted?.Invoke();
    }
}
