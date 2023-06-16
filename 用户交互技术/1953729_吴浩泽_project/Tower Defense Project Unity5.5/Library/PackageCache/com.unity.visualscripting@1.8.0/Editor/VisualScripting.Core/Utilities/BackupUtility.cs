using System;
using System.IO;
using Unity.VisualScripting.IonicZip;
using UnityEditor;

namespace Unity.VisualScripting
{
    public static class BackupUtility
    {
        public static void BackupAssetsFolder()
        {
            BackupAssetsFolder(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
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

                zip.AddDirectory(Paths.assets);

                var zipPath = Path.Combine(Paths.backups, fileName);

                VersionControlUtility.Unlock(zipPath);

                zip.Save(zipPath);

                EditorUtility.ClearProgressBar();
            }
        }
    }
}
