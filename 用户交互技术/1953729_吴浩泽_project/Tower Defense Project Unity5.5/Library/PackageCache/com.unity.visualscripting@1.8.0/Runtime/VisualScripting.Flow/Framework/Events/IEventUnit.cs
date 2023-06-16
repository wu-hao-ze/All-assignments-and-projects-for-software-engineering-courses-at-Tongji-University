using System;

namespace Unity.VisualScripting
{
    public interface IEventUnit : IUnit, IGraphEventListener
    {
        bool coroutine { get; }
    }
    public interface IGameObjectEventUnit : IEventUnit
    {
        Type MessageListenerType { get; }
    }
}
