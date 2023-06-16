using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public abstract class NesterStateWidget<TNesterState> : StateWidget<TNesterState>
        where TNesterState : class, INesterState
    {
        protected NesterStateWidget(StateCanvas canvas, TNesterState state) : base(canvas, state) { }

        protected override IEnumerable<DropdownOption> contextOptions
        {
            get
            {
                var childReference = reference.ChildReference(state, false);

                if (state.childGraph != null)
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
            if (state.graph.zoom == 1)
            {
                var childReference = reference.ChildReference(state, false);

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
