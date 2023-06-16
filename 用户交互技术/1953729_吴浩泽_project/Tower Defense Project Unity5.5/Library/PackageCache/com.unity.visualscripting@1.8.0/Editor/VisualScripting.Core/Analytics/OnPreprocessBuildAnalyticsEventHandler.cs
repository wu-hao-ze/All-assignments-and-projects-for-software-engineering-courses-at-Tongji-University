using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Unity.VisualScripting.Analytics
{
    internal class OnPreprocessBuildAnalyticsEventHandler : UnityEditor.Build.IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!EditorAnalytics.enabled || !VSUsageUtility.isVisualScriptingUsed)
                return;

            OnPreprocessBuildAnalytics.Send(new OnPreprocessBuildAnalytics.Data()
            {
                guid = report.summary.guid.ToString(),
                buildTarget = report.summary.platform,
                buildTargetGroup = report.summary.platformGroup
            });
        }
    }
}
