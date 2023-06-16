using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class IconExportUtility
    {
#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Export Type Icon...", priority = LudiqProduct.DeveloperToolsMenuPriority + 302)]
#endif
        public static void ExportTypeIcon()
        {
            var size = 16;

            var type = typeof(Animator);

            var restoreSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(size, size));
            var icon = type.Icon()[size];

            icon.filterMode = FilterMode.Point;
            var rt = RenderTexture.GetTemporary(icon.width, icon.height);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            Graphics.Blit(icon, rt);
            var readableIcon = new Texture2D(icon.width, icon.height);
            readableIcon.ReadPixels(new Rect(0, 0, icon.width, icon.height), 0, 0);
            readableIcon.Apply();
            RenderTexture.active = null;
            icon = readableIcon;

            File.WriteAllBytes($"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), type.CSharpFileName(false))}@{size}x.png", icon.EncodeToPNG());

            EditorGUIUtility.SetIconSize(restoreSize);
        }

#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Export Message Icon...", priority = LudiqProduct.DeveloperToolsMenuPriority + 302)]
#endif
        public static void ExportMessageIcon()
        {
            var size = 16;

            var type = MessageType.Warning;

            var restoreSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(size, size));

            var icon = LudiqGUIUtility.GetHelpIcon(type);

            icon.filterMode = FilterMode.Point;
            var rt = RenderTexture.GetTemporary(icon.width, icon.height);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            Graphics.Blit(icon, rt);
            var readableIcon = new Texture2D(icon.width, icon.height);
            readableIcon.ReadPixels(new Rect(0, 0, icon.width, icon.height), 0, 0);
            readableIcon.Apply();
            RenderTexture.active = null;
            icon = readableIcon;

            File.WriteAllBytes($"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), type.ToString())}@{size}x.png", icon.EncodeToPNG());

            EditorGUIUtility.SetIconSize(restoreSize);
        }

        private static Texture2D CreateReadableCopy(Texture2D source)
        {
            RenderTexture rt = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(source, rt);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);
            return readableText;
        }

#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Export All Editor Icons...", priority = LudiqProduct.DeveloperToolsMenuPriority + 302)]
#endif
        public static void ExportAllEditorIcon()
        {
            var assetBundle = typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, null) as AssetBundle;

            var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EditorTextures");

            PathUtility.CreateDirectoryIfNeeded(outputFolder);

            var textures = assetBundle.LoadAllAssets<Texture2D>();

            try
            {
                for (int i = 0; i < textures.Length; i++)
                {
                    var texture = textures[i];

                    try
                    {
                        ProgressUtility.DisplayProgressBar("Export Editor Textures", texture.name, (float)i / textures.Length);
                        var outputPath = Path.Combine(outputFolder, texture.name + ".png");
                        File.WriteAllBytes(outputPath, CreateReadableCopy(texture).EncodeToPNG());
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to export {texture.name}:\n{ex}");
                    }
                }
            }
            finally
            {
                ProgressUtility.ClearProgressBar();
            }
        }
    }
}
