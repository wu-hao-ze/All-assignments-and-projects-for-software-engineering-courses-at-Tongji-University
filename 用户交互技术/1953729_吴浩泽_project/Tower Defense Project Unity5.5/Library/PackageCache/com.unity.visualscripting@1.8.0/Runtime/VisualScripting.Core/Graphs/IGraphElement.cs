using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IGraphElement : IGraphItem, INotifiedCollectionItem, IDisposable, IPrewarmable, IAotStubbable, IIdentifiable, IAnalyticsIdentifiable
    {
        new IGraph graph { get; set; }

        bool HandleDependencies();

        int dependencyOrder { get; }

        new Guid guid { get; set; }

        void Instantiate(GraphReference instance);

        void Uninstantiate(GraphReference instance);

        IEnumerable<ISerializationDependency> deserializationDependencies { get; }
    }
}
