using System.IO;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

using Codice.Client.BaseCommands;
using Codice.Client.Common;

namespace Unity.PlasticSCM.Editor.AssetUtils
{
    internal static class SaveAssets
    {
        internal static void ForChangesWithConfirmation(
            List<ChangeInfo> changes,
            out bool isCancelled)
        {
            ForPaths(
                GetPaths(changes), true,
                out isCancelled);
        }

        internal static void ForPathsWithConfirmation(
            List<string> paths,
            out bool isCancelled)
        {
            ForPaths(
                paths, true,
                out isCancelled);
        }

        internal static void ForChangesWithoutConfirmation(
            List<ChangeInfo> changes)
        {
            bool isCancelled;
            ForPaths(
                GetPaths(changes), false,
                out isCancelled);
        }

        internal static void ForPathsWithoutConfirmation(
            List<string> paths)
        {
            bool isCancelled;
            ForPaths(
                paths, false,
                out isCancelled);
        }

        static void ForPaths(
            List<string> paths,
            bool askForUserConfirmation,
            out bool isCancelled)
        {
            SaveDirtyScenes(
                paths,
                askForUserConfirmation,
                out isCancelled);

            if (isCancelled)
                return;

            AssetDatabase.SaveAssets();
        }

        static void SaveDirtyScenes(
            List<string> paths,
            bool askForUserConfirmation,
            out bool isCancelled)
        {
            isCancelled = false;

            List<Scene> scenesToSave = new List<Scene>();

            foreach (Scene dirtyScene in GetDirtyScenes())
            {
                if (Contains(paths, dirtyScene))
                    scenesToSave.Add(dirtyScene);
            }

            if (scenesToSave.Count == 0)
                return;

            if (askForUserConfirmation)
            {
                isCancelled = !EditorSceneManager.
                    SaveModifiedScenesIfUserWantsTo(
                        scenesToSave.ToArray());
                return;
            }

            EditorSceneManager.SaveScenes(
                scenesToSave.ToArray());
        }

        static List<Scene> GetDirtyScenes()
        {
            List<Scene> dirtyScenes = new List<Scene>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (!scene.isDirty)
                    continue;

                dirtyScenes.Add(scene);
            }

            return dirtyScenes;
        }

        static bool Contains(
            List<string> paths,
            Scene scene)
        {
            foreach (string path in paths)
            {
                if (PathHelper.IsSamePath(
                        path,
                        Path.GetFullPath(scene.path)))
                    return true;
            }

            return false;
        }

        static List<string> GetPaths(List<ChangeInfo> changeInfos)
        {
            List<string> result = new List<string>();
            foreach (ChangeInfo change in changeInfos)
                result.Add(change.GetFullPath());
            return result;
        }
    }
}
