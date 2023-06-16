using System;
using UnityEngine;

namespace UnityEditor.Timeline
{
    // Helper methods for animated properties
    internal static class AnimatedPropertyUtility
    {
        public static bool IsMaterialProperty(string propertyName)
        {
            return propertyName.StartsWith("material.");
        }

        /// <summary>
        /// Given a propertyName (from an EditorCurveBinding), and the gameObject it refers to,
        /// remaps the path to include the exposed name of the shader parameter
        /// </summary>
        /// <param name="gameObject">The gameObject being referenced.</param>
        /// <param name="propertyName">The propertyName to remap.</param>
        /// <returns>The remapped propertyName, or the original propertyName if it cannot be remapped</returns>
        public static string RemapMaterialName(GameObject gameObject, string propertyName)
        {
            if (!IsMaterialProperty(propertyName) || gameObject == null)
                return propertyName;

            var renderers = gameObject.GetComponents<Renderer>();
            if (renderers == null || renderers.Length == 0)
                return propertyName;

            var propertySplits = propertyName.Split('.');
            if (propertySplits.Length <= 1)
                return propertyName;

            // handles post fixes for texture properties
            var exposedParameter = HandleTextureProperties(propertySplits[1], out var postFix);
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material.shader == null)
                        continue;

                    var index = material.shader.FindPropertyIndex(exposedParameter);
                    if (index >= 0)
                    {
                        propertySplits[1] = material.shader.GetPropertyDescription(index) + postFix;
                        return String.Join(".", propertySplits);
                    }
                }
            }

            return propertyName;
        }

        private static string HandleTextureProperties(string exposedParameter, out string postFix)
        {
            postFix = String.Empty;
            RemoveEnding(ref exposedParameter, ref postFix, "_ST");
            RemoveEnding(ref exposedParameter, ref postFix, "_TexelSize");
            RemoveEnding(ref exposedParameter, ref postFix, "_HDR");
            return exposedParameter;
        }

        private static void RemoveEnding(ref string name, ref string postFix, string ending)
        {
            if (name.EndsWith(ending))
            {
                name = name.Substring(0, name.Length - ending.Length);
                postFix = ending;
            }
        }
    }
}
