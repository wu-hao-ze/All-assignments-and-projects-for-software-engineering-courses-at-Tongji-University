using System.Linq;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(FlowStateTransition))]
    public class FlowStateTransitionDescriptor : NesterStateTransitionDescriptor<FlowStateTransition>
    {
        public FlowStateTransitionDescriptor(FlowStateTransition transition) : base(transition) { }

        public override string Title()
        {
            var graph = transition.nest.graph;

            if (graph != null)
            {
                if (!StringUtility.IsNullOrWhiteSpace(graph.title))
                {
                    return graph.title;
                }

                using (var recursion = Recursion.New(1))
                {
                    var events = graph.GetUnitsRecursive(recursion).OfType<IEventUnit>().ToList();

                    if (events.Count == 0)
                    {
                        return "(No Event)";
                    }
                    else if (events.Count == 1)
                    {
                        return events[0].Description().title;
                    }
                    else // if (events.Count > 1)
                    {
                        return "(Multiple Events)";
                    }
                }
            }
            else
            {
                return "(No Transition)";
            }
        }

        public override string Summary()
        {
            var graph = transition.nest.graph;

            if (graph != null)
            {
                if (!StringUtility.IsNullOrWhiteSpace(graph.summary))
                {
                    return graph.summary;
                }

                using (var recursion = Recursion.New(1))
                {
                    var events = graph.GetUnitsRecursive(recursion).OfType<IEventUnit>().ToList();

                    if (events.Count == 0)
                    {
                        return "Open the transition graph to add an event.";
                    }
                    else if (events.Count == 1)
                    {
                        return events[0].Description().summary;
                    }
                    else // if (events.Count > 1)
                    {
                        return "Open the transition graph to see the full transition.";
                    }
                }
            }
            else
            {
                return "Choose a source in the graph inspector.";
            }
        }

        public override EditorTexture Icon()
        {
            var graph = transition.nest.graph;

            using (var recursion = Recursion.New(1))
            {
                if (graph != null)
                {
                    var events = graph.GetUnitsRecursive(recursion).OfType<IEventUnit>().ToList();

                    if (events.Count == 1)
                    {
                        return events[0].Description().icon;
                    }
                    else
                    {
                        return typeof(IStateTransition).Icon();
                    }
                }
                else
                {
                    return typeof(IStateTransition).Icon();
                }
            }
        }
    }
}
