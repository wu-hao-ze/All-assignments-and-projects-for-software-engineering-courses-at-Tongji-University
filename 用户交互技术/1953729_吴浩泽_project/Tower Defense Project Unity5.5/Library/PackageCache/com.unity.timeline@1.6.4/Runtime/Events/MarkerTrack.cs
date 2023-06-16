using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
    /// <summary>
    /// Use this track to add Markers bound to a GameObject.
    /// </summary>
    [Serializable]
    [TrackBindingType(typeof(GameObject))]
    [HideInMenu]
    [ExcludeFromPreset]
    [TimelineHelpURL(typeof(MarkerTrack))]
    public class MarkerTrack : TrackAsset
    {
        /// <inheritdoc/>
        public override IEnumerable<PlayableBinding> outputs
        {
            get
            {
                return this == timelineAsset.markerTrack ?
                    new List<PlayableBinding> { ScriptPlayableBinding.Create(name, null, typeof(GameObject)) } :
                    base.outputs;
            }
        }
    }
}
