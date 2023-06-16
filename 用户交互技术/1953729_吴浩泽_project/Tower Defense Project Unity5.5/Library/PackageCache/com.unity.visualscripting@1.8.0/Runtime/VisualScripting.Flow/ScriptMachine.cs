using UnityEngine;

namespace Unity.VisualScripting
{
    [AddComponentMenu("Visual Scripting/Script Machine")]
    [RequireComponent(typeof(Variables))]
    [DisableAnnotation]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.visualscripting@latest/index.html?subfolder=/manual/vs-graphs-machines-macros.html")]
    [RenamedFrom("Bolt.FlowMachine")]
    [RenamedFrom("Unity.VisualScripting.FlowMachine")]
    public sealed class ScriptMachine : EventMachine<FlowGraph, ScriptGraphAsset>
    {
        public override FlowGraph DefaultGraph()
        {
            return FlowGraph.WithStartUpdate();
        }

        protected override void OnEnable()
        {
            if (hasGraph)
            {
                graph.StartListening(reference);
            }

            base.OnEnable();
        }

        protected override void OnInstantiateWhileEnabled()
        {
            if (hasGraph)
            {
                graph.StartListening(reference);
            }

            base.OnInstantiateWhileEnabled();
        }

        protected override void OnUninstantiateWhileEnabled()
        {
            base.OnUninstantiateWhileEnabled();

            if (hasGraph)
            {
                graph.StopListening(reference);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (hasGraph)
            {
                graph.StopListening(reference);
            }
        }

        [ContextMenu("Show Data...")]
        protected override void ShowData()
        {
            base.ShowData();
        }
    }
}
