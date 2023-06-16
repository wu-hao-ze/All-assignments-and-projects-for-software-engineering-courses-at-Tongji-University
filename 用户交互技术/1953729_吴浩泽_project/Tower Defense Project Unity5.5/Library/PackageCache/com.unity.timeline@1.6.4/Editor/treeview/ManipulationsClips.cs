using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace UnityEditor.Timeline
{
    class DrillIntoClip : Manipulator
    {
        protected override bool DoubleClick(Event evt, WindowState state)
        {
            if (evt.button != 0)
                return false;

            var guiClip = PickerUtils.TopmostPickedItem() as TimelineClipGUI;

            if (guiClip == null)
                return false;

            if (!TimelineWindow.instance.state.editSequence.isReadOnly && (guiClip.clip.curves != null || guiClip.clip.animationClip != null))
                Invoker.Invoke<EditClipInAnimationWindow>(new[] { guiClip.clip });

            if (guiClip.supportsSubTimelines)
                Invoker.Invoke<EditSubTimeline>(new[] { guiClip.clip });

            return true;
        }
    }

    class ContextMenuManipulator : Manipulator
    {
        protected override bool MouseDown(Event evt, WindowState state)
        {
            if (evt.button == 1)
                ItemSelection.HandleSingleSelection(evt);

            return false;
        }

        protected override bool ContextClick(Event evt, WindowState state)
        {
            if (evt.alt)
                return false;

            var selectable = PickerUtils.TopmostPickedItem() as ISelectable;

            if (selectable != null && selectable.IsSelected())
            {
                SequencerContextMenu.ShowItemContextMenu(evt.mousePosition);
                return true;
            }

            return false;
        }
    }
}
