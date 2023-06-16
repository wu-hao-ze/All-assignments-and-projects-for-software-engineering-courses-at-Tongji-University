using System;

namespace Unity.VisualScripting
{
    public interface IGraphNest : IAotStubbable
    {
        IGraphNester nester { get; set; }

        GraphSource source { get; set; }
        IGraph embed { get; set; }
        IMacro macro { get; set; }
        IGraph graph { get; }

        Type graphType { get; }
        Type macroType { get; }

        bool hasBackgroundEmbed { get; }
    }
}
