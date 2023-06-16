using System.Linq;
using UnityEditor;

namespace Unity.VisualScripting
{
    public class AnnotationDisabler
    {
#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Disable Gizmos", priority = LudiqProduct.DeveloperToolsMenuPriority + 502)]
#endif
        public static void DisableGizmos()
        {
            foreach (var type in Codebase.types.Where(type => type.HasAttribute<DisableAnnotationAttribute>()))
            {
                var attribute = type.GetAttribute<DisableAnnotationAttribute>();

                var annotation = AnnotationUtility.GetAnnotation(type);

                if (annotation != null)
                {
                    annotation.iconEnabled = !attribute.disableIcon;
                    annotation.gizmoEnabled = !attribute.disableGizmo;
                }
            }
        }
    }
}
