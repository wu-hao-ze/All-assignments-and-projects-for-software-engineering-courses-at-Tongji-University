using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    [UsedImplicitly]
    class CopyMarkersToClipboard : MarkerAction
    {
        public override ActionValidity Validate(IEnumerable<IMarker> markers) => ActionValidity.Valid;

        public override bool Execute(IEnumerable<IMarker> markers)
        {
            TimelineEditor.clipboard.CopyItems(markers.ToItems());
            return true;
        }
    }
}
