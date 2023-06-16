using System;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting.IonicZip;
using System.IO;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class VSBackupUtility
    {
        public static void Backup()
        {
            BackupAssetsFolder(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
        }

        public static List<string> Find<T>() where T : UnityEngine.Object
        {
            List<string> assets = new List<string>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);

                assets.Add(assetPath);
            }

            return assets;
        }

        public static void BackupAssetsFolder(string backupLabel)
        {
            backupLabel = PathUtility.MakeSafeFilename(backupLabel, '_');

            PathUtility.CreateDirectoryIfNeeded(Paths.backups);

            var fileName = $"Assets_{backupLabel}.zip";

            var addEntryIndex = 0;
            var saveEntryIndex = 0;

            using (var zip = new ZipFile())
            {
                zip.UseZip64WhenSaving = Zip64Option.AsNecessary;

                zip.AddProgress += (sender, e) => { EditorUtility.DisplayProgressBar("Creating Backup...", e.CurrentEntry != null ? e.CurrentEntry.FileName : "...", (float)(addEntryIndex++) / e.EntriesTotal); };

                zip.SaveProgress += (sender, e) => { EditorUtility.DisplayProgressBar("Creating Backup...", e.CurrentEntry != null ? e.CurrentEntry.FileName : "...", (float)(saveEntryIndex++) / e.EntriesTotal); };

                List<string> listOfAssets = Find<LudiqScriptableObject>();

                foreach (string assetPath in listOfAssets)
                {
                    zip.AddFile(assetPath);
                }

                var zipPath = Path.Combine(Paths.backups, fileName);

                VersionControlUtility.Unlock(zipPath);

                zip.Save(zipPath);

                Debug.Log($"Visual Scripting Migration: A backup of all Visual Scripting related assets has been created at {zipPath}");

                EditorUtility.ClearProgressBar();
            }
        }
    }
}
