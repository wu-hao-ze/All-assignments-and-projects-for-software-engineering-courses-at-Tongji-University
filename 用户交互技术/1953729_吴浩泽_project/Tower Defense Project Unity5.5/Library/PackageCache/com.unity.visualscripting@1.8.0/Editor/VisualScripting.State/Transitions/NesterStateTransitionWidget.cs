using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public abstract class NesterStateTransitionWidget<TNesterStateTransition> : StateTransitionWidget<TNesterStateTransition>
        where TNesterStateTransition : class, INesterStateTransition
    {
        protected NesterStateTransitionWidget(StateCanvas canvas, TNesterStateTransition transition) : base(canvas, transition) { }

        protected override IEnumerable<DropdownOption> contextOptions
        {
            get
            {
                var childReference = reference.ChildReference(transition, false);

                if (childReference != null)
                {
                    yield return new DropdownOption((Action)(() => window.reference = childReference), "Open");
                    yield return new DropdownOption((Action)(() => GraphWindow.OpenTab(childReference)), "Open in new window");
                }

                foreach (var baseOption in base.contextOptions)
                {
                    yield return baseOption;
                }
            }
        }

        protected override void OnDoubleClick()
        {
            if (transition.graph.zoom == 1)
            {
                var childReference = reference.ChildReference(transition, false);

                if (childReference != null)
                {
                    if (e.ctrlOrCmd)
                    {
                        GraphWindow.OpenTab(childReference);
                    }
                    else
                    {
                        window.reference = childReference;
                    }
                }

                e.Use();
            }
            else
            {
                base.OnDoubleClick();
            }
        }
    }
}
