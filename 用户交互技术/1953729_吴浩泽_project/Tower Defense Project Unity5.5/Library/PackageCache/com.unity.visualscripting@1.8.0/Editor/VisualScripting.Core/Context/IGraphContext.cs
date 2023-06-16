using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IGraphContext : IDisposable
    {
        GraphReference reference { get; }

        IEnumerable<IGraphContextExtension> extensions { get; }

        IGraph graph { get; }

        ICanvas canvas { get; }

        GraphSelection selection { get; }

        Metadata graphMetadata { get; }

        Metadata selectionMetadata { get; }

        Metadata ElementMetadata(IGraphElement element);

        AnalyserProvider analyserProvider { get; }

        string windowTitle { get; }

        IEnumerable<ISidebarPanelContent> sidebarPanels { get; }

        bool isPrefabInstance { get; }

        void BeginEdit(bool disablePrefabInstance = true);

        void EndEdit();

        void DescribeAndAnalyze();
    }
}
