using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor.Timeline.Signals
{
    class SignalReceiverHeader : MultiColumnHeader
    {
        public SignalReceiverHeader(MultiColumnHeaderState state) : base(state) { }

        protected override void AddColumnHeaderContextMenuItems(GenericMenu menu)
        {
            menu.AddItem(L10n.TextContent("Resize to Fit"), false, ResizeToFit);
        }
    }
}
